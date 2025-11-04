using Microsoft.AspNetCore.Mvc;
using comaiz.data;
using comaiz.data.Models;
using Microsoft.EntityFrameworkCore;

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

            var query = dbContext.Tasks.AsQueryable();
            
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

            var task = await dbContext.Tasks.FindAsync(id);

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

            dbContext.Entry(task).State = EntityState.Modified;

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
