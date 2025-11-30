using comaiz.data;
using Microsoft.AspNetCore.Mvc;
using comaiz.data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace comaiz.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserContractRatesController : ControllerBase
    {
        private readonly ComaizContext dbContext;

        public UserContractRatesController(ComaizContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserContractRate>>> GetUserContractRates([FromQuery] int? contractRateId, [FromQuery] string? applicationUserId)
        {
            if(dbContext.UserContractRates == null) return StatusCode(StatusCodes.Status500InternalServerError);

            var query = dbContext.UserContractRates
                .Include(ucr => ucr.ContractRate)
                .Include(ucr => ucr.ApplicationUser)
                .AsQueryable();
            
            if (contractRateId.HasValue)
            {
                query = query.Where(ucr => ucr.ContractRateId == contractRateId.Value);
            }

            if (!string.IsNullOrEmpty(applicationUserId))
            {
                query = query.Where(ucr => ucr.ApplicationUserId == applicationUserId);
            }

            return await query.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserContractRate>> GetUserContractRate(int id)
        {
            if (dbContext.UserContractRates == null) return StatusCode(StatusCodes.Status500InternalServerError);

            var userContractRate = await dbContext.UserContractRates
                .Include(ucr => ucr.ContractRate)
                .Include(ucr => ucr.ApplicationUser)
                .FirstOrDefaultAsync(ucr => ucr.Id == id);

            if (userContractRate == null)
            {
                return NotFound();
            }

            return userContractRate;
        }

        [HttpPut]
        public async Task<IActionResult> PutUserContractRate(UserContractRate userContractRate)
        {
            if (dbContext.UserContractRates == null) return StatusCode(StatusCodes.Status500InternalServerError);

            dbContext.Entry(userContractRate).State = EntityState.Modified;

            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserContractRateExists(userContractRate.Id))
                {
                    return NotFound();
                }
            }

            return NoContent();
        }

        private bool UserContractRateExists(int id)
        {
            if (dbContext.UserContractRates == null) return false;

            return dbContext.UserContractRates.Any(e => e.Id == id);
        }

        [HttpPost]
        public async Task<ActionResult<UserContractRate>> PostUserContractRate(UserContractRate userContractRate)
        {
            if (dbContext.UserContractRates == null) return StatusCode(StatusCodes.Status500InternalServerError);

            dbContext.UserContractRates.Add(userContractRate);
            await dbContext.SaveChangesAsync();

            return CreatedAtAction("GetUserContractRate", new { id = userContractRate.Id }, userContractRate);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserContractRate(int id)
        {
            if (dbContext.UserContractRates == null) return StatusCode(StatusCodes.Status500InternalServerError);

            var userContractRate = await dbContext.UserContractRates.FindAsync(id);
            if (userContractRate == null)
            {
                return NotFound();
            }

            dbContext.UserContractRates.Remove(userContractRate);
            await dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
