using Microsoft.AspNetCore.Mvc;
using comaiz.data;
using comaiz.data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;


namespace comaiz.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        public readonly ComaizContext dbContext;
        public InvoicesController(ComaizContext context)
        {
            dbContext = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Invoice>>> GetInvoices([FromQuery] int? contractId)
        {
            if (dbContext.Invoices == null) return StatusCode(StatusCodes.Status500InternalServerError);

            var query = dbContext.Invoices.AsQueryable();
            
            if (contractId.HasValue)
            {
                // Filter invoices by the client associated with the contract
                var contract = await dbContext.Contracts
                    .Where(c => c.Id == contractId.Value)
                    .Select(c => new { c.ClientId })
                    .FirstOrDefaultAsync();
                
                if (contract == null)
                {
                    // If contract not found, return empty list
                    return new List<Invoice>();
                }
                
                query = query.Where(i => i.ClientId == contract.ClientId);
            }

            return await query.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Invoice>> GetInvoice(int id)
        {
            if (dbContext.Invoices == null) return StatusCode(StatusCodes.Status500InternalServerError);

            var invoice = await dbContext.Invoices.FindAsync(id);

            if (invoice == null)
            {
                return NotFound();
            }

            return invoice;
        }

        [HttpPut]
        public async Task<IActionResult> PutInvoice(Invoice invoice)
        {
            if (dbContext.Invoices == null) return StatusCode(StatusCodes.Status500InternalServerError);

            dbContext.Entry(invoice).State = EntityState.Modified;

            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InvoiceExists(invoice.Id))
                {
                    return NotFound();
                }
            }

            return NoContent();
        }

        private bool InvoiceExists(int id)
        { 
            if (dbContext.Invoices == null) return false;

            return dbContext.Invoices.Any(e => e.Id == id);
        }

        [HttpPost]
        public async Task<ActionResult<Invoice>> PostInvoice(Invoice invoice)
        {
            if (dbContext.Invoices == null) return StatusCode(StatusCodes.Status500InternalServerError);

            dbContext.Invoices.Add(invoice);
            await dbContext.SaveChangesAsync();

            return CreatedAtAction("GetInvoice", new { id = invoice.Id }, invoice);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            if (dbContext.Invoices == null) return StatusCode(StatusCodes.Status500InternalServerError);

            var invoice = await dbContext.Invoices.FindAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }

            dbContext.Invoices.Remove(invoice);
            await dbContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{id}/duplicate")]
        public async Task<ActionResult<Invoice>> DuplicateInvoice(int id)
        {
            if (dbContext.Invoices == null) return StatusCode(StatusCodes.Status500InternalServerError);

            var invoice = await dbContext.Invoices
                .Include(i => i.InvoiceItems)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (invoice == null)
            {
                return NotFound();
            }

            var duplicatedInvoice = new Invoice
            {
                Date = invoice.Date,
                PurchaseOrder = invoice.PurchaseOrder,
                ClientId = invoice.ClientId,
                State = InvoiceState.Draft // Duplicated invoices always start as Draft
            };

            // Copy invoice items if they exist
            if (invoice.InvoiceItems != null && invoice.InvoiceItems.Any())
            {
                foreach (var item in invoice.InvoiceItems)
                {
                    duplicatedInvoice.InvoiceItems ??= new List<InvoiceItem>();
                    duplicatedInvoice.InvoiceItems.Add(new InvoiceItem
                    {
                        TaskId = item.TaskId,
                        FixedCostId = item.FixedCostId,
                        Quantity = item.Quantity,
                        Unit = item.Unit,
                        Rate = item.Rate,
                        VATRate = item.VATRate,
                        Price = item.Price,
                        StartDate = item.StartDate,
                        EndDate = item.EndDate
                    });
                }
            }

            dbContext.Invoices.Add(duplicatedInvoice);
            await dbContext.SaveChangesAsync();

            return CreatedAtAction("GetInvoice", new { id = duplicatedInvoice.Id }, duplicatedInvoice);
        }
    }
}
