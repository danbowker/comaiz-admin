using comaiz.data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using comaiz.data.Models;
using Microsoft.EntityFrameworkCore;

namespace comaiz.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractRatesController : ControllerBase
    {
        private readonly ComaizContext dbContext;

        public ContractRatesController(ComaizContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContractRate>>> GetContractRates()
        {
            return await dbContext.ContractRates.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ContractRate>> GetContractRate(int id)
        {
            var contractRate = await dbContext.ContractRates.FindAsync(id);

            if (contractRate == null)
            {
                return NotFound();
            }

            return contractRate;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutContractRate(int id, ContractRate contractRate)
        {
            if (id != contractRate.Id)
            {
                return BadRequest();
            }

            dbContext.Entry(contractRate).State = EntityState.Modified;

            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContractRateExists(id))
                {
                    return NotFound();
                }
            }

            return NoContent();
        }

        private bool ContractRateExists(int id)
        {
            return dbContext.ContractRates.Any(e => e.Id == id);
        }

        [HttpPost]
        public async Task<ActionResult<ContractRate>> PostContractRate(ContractRate contractRate)
        {
            dbContext.ContractRates.Add(contractRate);
            await dbContext.SaveChangesAsync();

            return CreatedAtAction("GetContractRate", new { id = contractRate.Id }, contractRate);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContractRate(int id)
        {
            var contractRate = await dbContext.ContractRates.FindAsync(id);
            if (contractRate == null)
            {
                return NotFound();
            }

            dbContext.ContractRates.Remove(contractRate);
            await dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
