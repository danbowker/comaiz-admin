using Microsoft.AspNetCore.Http;
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
            return await dbContext.Contracts.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Contract>> GetContractAsync(int id)
        {
            var contract = await dbContext.Contracts.FindAsync(id);

            if (contract == null)
            {
                return NotFound();
            }

            return contract;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutContract(int id, Contract contract)
        {
            if (id != contract.Id)
            {
                return BadRequest();
            }

            dbContext.Entry(contract).State = EntityState.Modified;

            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContractExists(id))
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
            return dbContext.Contracts.Any(e => e.Id == id);
        }

        [HttpPost]
        public async Task<ActionResult<Contract>> PostContract(Contract contract)
        {
            dbContext.Contracts.Add(contract);
            await dbContext.SaveChangesAsync();

            return CreatedAtAction("GetContract", new { id = contract.Id }, contract);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContract(int id)
        {
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
