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
        public async Task<ActionResult<IEnumerable<Invoice>>> GetInvoices()
        {
            if (dbContext.Invoices == null) return StatusCode(StatusCodes.Status500InternalServerError);

            return await dbContext.Invoices.ToListAsync();
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
    }
}
