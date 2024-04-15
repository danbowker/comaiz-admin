using comaiz.data;
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
            if(dbContext.ContractRates == null) return StatusCode(StatusCodes.Status500InternalServerError);

            return await dbContext.ContractRates.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ContractRate>> GetContractRate(int id)
        {
            if (dbContext.ContractRates == null) return StatusCode(StatusCodes.Status500InternalServerError);

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

            if (dbContext.ContractRates == null) return StatusCode(StatusCodes.Status500InternalServerError);

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
            if (dbContext.ContractRates == null) return false;

            return dbContext.ContractRates.Any(e => e.Id == id);
        }

        [HttpPost]
        public async Task<ActionResult<ContractRate>> PostContractRate(ContractRate contractRate)
        {
            if (dbContext.ContractRates == null) return StatusCode(StatusCodes.Status500InternalServerError);

            dbContext.ContractRates.Add(contractRate);
            await dbContext.SaveChangesAsync();

            return CreatedAtAction("GetContractRate", new { id = contractRate.Id }, contractRate);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContractRate(int id)
        {
            if (dbContext.ContractRates == null) return StatusCode(StatusCodes.Status500InternalServerError);

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
