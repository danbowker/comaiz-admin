using Microsoft.AspNetCore.Mvc;
using comaiz.data;
using comaiz.data.Models;
using Microsoft.EntityFrameworkCore;

namespace comaiz.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractsController : ControllerBase
    {
        private readonly ComaizContext dbContext;

        public ContractsController(ComaizContext context)
        {
            dbContext = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contract>>> GetContractsAsync()
        {
            if (dbContext.Contracts == null) return StatusCode(StatusCodes.Status500InternalServerError);

            return await dbContext.Contracts.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Contract>> GetContractAsync(int id)
        {
            if (dbContext.Contracts == null) return StatusCode(StatusCodes.Status500InternalServerError);

            var contract = await dbContext.Contracts.FindAsync(id);

            if (contract == null)
            {
                return NotFound();
            }

            return contract;
        }

        [HttpPut]
        public async Task<IActionResult> PutContract(Contract contract)
        {
            if (dbContext.Contracts == null) return StatusCode(StatusCodes.Status500InternalServerError);

            dbContext.Entry(contract).State = EntityState.Modified;

            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContractExists(contract.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool ContractExists(int id)
        {
            if (dbContext.Contracts == null) return false;
            return dbContext.Contracts.Any(e => e.Id == id);
        }

        [HttpPost]
        public async Task<ActionResult<Contract>> PostContract(Contract contract)
        {
            if (dbContext.Contracts == null) return StatusCode(StatusCodes.Status500InternalServerError);

            dbContext.Contracts.Add(contract);
            await dbContext.SaveChangesAsync();

            return CreatedAtAction("GetContract", new { id = contract.Id }, contract);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContract(int id)
        {
            if(dbContext.Contracts == null) return StatusCode(StatusCodes.Status500InternalServerError);
            
            var contract = await dbContext.Contracts.FindAsync(id);
            if (contract == null)
            {
                return NotFound();
            }

            dbContext.Contracts.Remove(contract);
            await dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
