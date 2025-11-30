using Microsoft.AspNetCore.Mvc;
using comaiz.data;
using comaiz.data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace comaiz.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FixedCostsController : ControllerBase
    {
        private readonly ComaizContext dbContext;
        public FixedCostsController(ComaizContext context)
        {
            dbContext = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cost>>> GetCosts([FromQuery] int? contractId)
        {
            if (dbContext.FixedCosts == null) return StatusCode(StatusCodes.Status500InternalServerError);

            var query = dbContext.FixedCosts.AsQueryable();
            
            if (contractId.HasValue)
            {
                query = query.Where(fc => fc.ContractId == contractId.Value);
            }

            return await query.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Cost>> GetCost(int id)
        {
            if (dbContext.FixedCosts == null) return StatusCode(StatusCodes.Status500InternalServerError);

            var cost = await dbContext.FixedCosts.FindAsync(id);

            if (cost == null)
            {
                return NotFound();
            }

            return cost;
        }

        [HttpPut]
        public async Task<IActionResult> PutCost(Cost cost)
        {
            if (dbContext.FixedCosts == null) return StatusCode(StatusCodes.Status500InternalServerError);

            dbContext.Entry(cost).State = EntityState.Modified;

            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CostExists(cost.Id))
                {
                    return NotFound();
                }
            }

            return NoContent();
        }

        private bool CostExists(int id)
        {
            if (dbContext.FixedCosts == null) return false;

            return dbContext.FixedCosts.Any(e => e.Id == id);
        }

        [HttpPost]
        public async Task<ActionResult<Cost>> PostCost(FixedCost cost)
        {
            if (dbContext.FixedCosts == null) return StatusCode(StatusCodes.Status500InternalServerError);

            dbContext.FixedCosts.Add(cost);
            await dbContext.SaveChangesAsync();

            return CreatedAtAction("GetCost", new { id = cost.Id }, cost);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCost(int id)
        {
            if (dbContext.FixedCosts == null) return StatusCode(StatusCodes.Status500InternalServerError);

            var cost = await dbContext.FixedCosts.FindAsync(id);
            if (cost == null)
            {
                return NotFound();
            }

            dbContext.FixedCosts.Remove(cost);
            await dbContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{id}/duplicate")]
        public async Task<ActionResult<Cost>> DuplicateCost(int id)
        {
            if (dbContext.FixedCosts == null) return StatusCode(StatusCodes.Status500InternalServerError);

            var cost = await dbContext.FixedCosts.FindAsync(id);
            if (cost == null)
            {
                return NotFound();
            }

            var duplicatedCost = new FixedCost
            {
                ContractId = cost.ContractId,
                Name = cost.Name != null ? $"{cost.Name} (Copy)" : null,
                Amount = cost.Amount,
                InvoiceItemId = null // Don't copy the invoice item association
            };

            dbContext.FixedCosts.Add(duplicatedCost);
            await dbContext.SaveChangesAsync();

            return CreatedAtAction("GetCost", new { id = duplicatedCost.Id }, duplicatedCost);
        }
    }
}
