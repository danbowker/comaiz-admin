using Microsoft.AspNetCore.Mvc;
using comaiz.data;
using comaiz.data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;


namespace comaiz.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceItemsController : ControllerBase
    {
        private readonly ComaizContext dbContext;

        public InvoiceItemsController(ComaizContext context)
        {
            dbContext = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InvoiceItem>>> GetInvoiceItems([FromQuery] int? contractId)
        {
            if (dbContext.InvoiceItems == null) return StatusCode(StatusCodes.Status500InternalServerError);

            var query = dbContext.InvoiceItems.AsQueryable();
            
            if (contractId.HasValue)
            {
                query = query.Where(ii => 
                    (ii.Task != null && ii.Task.ContractId == contractId.Value) ||
                    (ii.FixedCost != null && ii.FixedCost.ContractId == contractId.Value)
                );
            }

            return await query.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InvoiceItem>> GetInvoiceItem(int id)
        {
            if (dbContext.InvoiceItems == null) return StatusCode(StatusCodes.Status500InternalServerError);

            var invoiceItem = await dbContext.InvoiceItems.FindAsync(id);

            if (invoiceItem == null)
            {
                return NotFound();
            }

            return invoiceItem;
        }

        [HttpPut]
        public async Task<IActionResult> PutInvoiceItem(InvoiceItem invoiceItem)
        {
            if (dbContext.InvoiceItems == null) return StatusCode(StatusCodes.Status500InternalServerError);

            dbContext.Entry(invoiceItem).State = EntityState.Modified;

            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InvoiceItemExists(invoiceItem.Id))
                {
                    return NotFound();
                }
            }

            return NoContent();
        }

        private bool InvoiceItemExists(int id)
        {
            if (dbContext.InvoiceItems == null) return false;

            return dbContext.InvoiceItems.Any(e => e.Id == id);
        }

        [HttpPost]
        public async Task<ActionResult<InvoiceItem>> PostInvoiceItem(InvoiceItem invoiceItem)
        {
            if(dbContext.InvoiceItems == null) return StatusCode(StatusCodes.Status500InternalServerError);

            dbContext.InvoiceItems.Add(invoiceItem);
            await dbContext.SaveChangesAsync();

            return CreatedAtAction("GetInvoiceItem", new { id = invoiceItem.Id }, invoiceItem);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoiceItem(int id)
        {
            if (dbContext.InvoiceItems == null) return StatusCode(StatusCodes.Status500InternalServerError);

            var invoiceItem = await dbContext.InvoiceItems.FindAsync(id);
            if (invoiceItem == null)
            {
                return NotFound();
            }

            dbContext.InvoiceItems.Remove(invoiceItem);
            await dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
