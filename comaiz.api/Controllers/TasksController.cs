using Microsoft.AspNetCore.Mvc;
using comaiz.data;
using comaiz.data.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace comaiz.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ComaizContext dbContext;

        public TasksController(ComaizContext context)
        {
            dbContext = context;
        }

        [HttpGet]
        public async System.Threading.Tasks.Task<ActionResult<IEnumerable<comaiz.data.Models.Task>>> GetTasks([FromQuery] int? contractId)
        {
            if (dbContext.Tasks == null) return StatusCode(StatusCodes.Status500InternalServerError);

            var query = dbContext.Tasks
                .Include(t => t.TaskContractRates!)
                    .ThenInclude(tcr => tcr.ContractRate)
                .AsQueryable();
            
            if (contractId.HasValue)
            {
                query = query.Where(t => t.ContractId == contractId.Value);
            }

            return await query.ToListAsync();
        }

        [HttpGet("{id}")]
        public async System.Threading.Tasks.Task<ActionResult<comaiz.data.Models.Task>> GetTask(int id)
        {
            if (dbContext.Tasks == null) return StatusCode(StatusCodes.Status500InternalServerError);

            var task = await dbContext.Tasks
                .Include(t => t.TaskContractRates!)
                    .ThenInclude(tcr => tcr.ContractRate)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                return NotFound();
            }

            return task;
        }

        [HttpPut]
        public async System.Threading.Tasks.Task<IActionResult> PutTask(comaiz.data.Models.Task task)
        {
            if (dbContext.Tasks == null) return StatusCode(StatusCodes.Status500InternalServerError);

            // Load the existing task with its contract rates
            var existingTask = await dbContext.Tasks
                .Include(t => t.TaskContractRates)
                .FirstOrDefaultAsync(t => t.Id == task.Id);

            if (existingTask == null)
            {
                return NotFound();
            }

            // Update simple properties
            existingTask.Name = task.Name;
            existingTask.ContractId = task.ContractId;
            existingTask.ContractRateId = task.ContractRateId;

            // Update TaskContractRates collection
            if (existingTask.TaskContractRates != null)
            {
                // Remove existing contract rates
                dbContext.TaskContractRates!.RemoveRange(existingTask.TaskContractRates);
            }

            // Add new contract rates
            if (task.TaskContractRates != null && task.TaskContractRates.Any())
            {
                foreach (var tcr in task.TaskContractRates)
                {
                    // Create new entity with only foreign keys to avoid inserting navigation properties
                    var newTaskContractRate = new TaskContractRate
                    {
                        TaskId = existingTask.Id,
                        ContractRateId = tcr.ContractRateId
                    };
                    dbContext.TaskContractRates!.Add(newTaskContractRate);
                }
            }

            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskExists(task.Id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        private bool TaskExists(int id)
        {
            if (dbContext.Tasks == null) return false;

            return dbContext.Tasks.Any(e => e.Id == id);
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult<comaiz.data.Models.Task>> PostTask(comaiz.data.Models.Task task)
        {
            if (dbContext.Tasks == null) return StatusCode(StatusCodes.Status500InternalServerError);

            // Clean up navigation properties in TaskContractRates to avoid inserting related entities
            if (task.TaskContractRates != null && task.TaskContractRates.Any())
            {
                var contractRateIds = task.TaskContractRates.Select(tcr => tcr.ContractRateId).ToList();
                task.TaskContractRates.Clear();
                
                foreach (var contractRateId in contractRateIds)
                {
                    task.TaskContractRates.Add(new TaskContractRate
                    {
                        ContractRateId = contractRateId
                    });
                }
            }

            dbContext.Tasks.Add(task);
            await dbContext.SaveChangesAsync();

            return CreatedAtAction("GetTask", new { id = task.Id }, task);
        }

        [HttpDelete("{id}")]
        public async System.Threading.Tasks.Task<IActionResult> DeleteTask(int id)
        {
            if (dbContext.Tasks == null) return StatusCode(StatusCodes.Status500InternalServerError);

            var task = await dbContext.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            dbContext.Tasks.Remove(task);
            await dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
