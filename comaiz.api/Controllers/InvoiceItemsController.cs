﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using comaiz.data;
using comaiz.data.Models;
using Microsoft.EntityFrameworkCore;


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
        public async Task<ActionResult<IEnumerable<InvoiceItem>>> GetInvoiceItems()
        {
            return await dbContext.InvoiceItems.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InvoiceItem>> GetInvoiceItem(int id)
        {
            var invoiceItem = await dbContext.InvoiceItems.FindAsync(id);

            if (invoiceItem == null)
            {
                return NotFound();
            }

            return invoiceItem;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutInvoiceItem(int id, InvoiceItem invoiceItem)
        {
            if (id != invoiceItem.Id)
            {
                return BadRequest();
            }

            dbContext.Entry(invoiceItem).State = EntityState.Modified;

            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InvoiceItemExists(id))
                {
                    return NotFound();
                }
            }

            return NoContent();
        }

        private bool InvoiceItemExists(int id)
        {
            return dbContext.InvoiceItems.Any(e => e.Id == id);
        }

        [HttpPost]
        public async Task<ActionResult<InvoiceItem>> PostInvoiceItem(InvoiceItem invoiceItem)
        {
            dbContext.InvoiceItems.Add(invoiceItem);
            await dbContext.SaveChangesAsync();

            return CreatedAtAction("GetInvoiceItem", new { id = invoiceItem.Id }, invoiceItem);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoiceItem(int id)
        {
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
