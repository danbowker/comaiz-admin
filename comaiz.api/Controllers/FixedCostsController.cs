using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using comaiz.data;
using comaiz.data.Models;
using Microsoft.EntityFrameworkCore;

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
        public async Task<ActionResult<IEnumerable<Cost>>> GetCosts()
        {
            return await dbContext.FixedCosts.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Cost>> GetCost(int id)
        {
            var cost = await dbContext.FixedCosts.FindAsync(id);

            if (cost == null)
            {
                return NotFound();
            }

            return cost;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCost(int id, Cost cost)
        {
            if (id != cost.Id)
            {
                return BadRequest();
            }

            dbContext.Entry(cost).State = EntityState.Modified;

            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CostExists(id))
                {
                    return NotFound();
                }
            }

            return NoContent();
        }

        private bool CostExists(int id)
        {
            return dbContext.FixedCosts.Any(e => e.Id == id);
        }

        [HttpPost]
        public async Task<ActionResult<Cost>> PostCost(FixedCost cost)
        {
            dbContext.FixedCosts.Add(cost);
            await dbContext.SaveChangesAsync();

            return CreatedAtAction("GetCost", new { id = cost.Id }, cost);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCost(int id)
        {
            var cost = await dbContext.FixedCosts.FindAsync(id);
            if (cost == null)
            {
                return NotFound();
            }

            dbContext.FixedCosts.Remove(cost);
            await dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
