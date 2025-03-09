using Microsoft.AspNetCore.Mvc;
using comaiz.data;
using comaiz.data.Models;
using Microsoft.EntityFrameworkCore;


namespace comaiz.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkersController : ControllerBase
    {
        public readonly ComaizContext dbContext;

        public WorkersController(ComaizContext context)
        {
            dbContext = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Worker>>> GetWorkers()
        {
            if (dbContext.Workers == null) return StatusCode(StatusCodes.Status500InternalServerError);

            return await dbContext.Workers.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Worker>> GetWorker(int id)
        {
            if (dbContext.Workers == null) return StatusCode(StatusCodes.Status500InternalServerError);

            var worker = await dbContext.Workers.FindAsync(id);

            if (worker == null)
            {
                return NotFound();
            }

            return worker;
        }

        [HttpPut]
        public async Task<IActionResult> PutWorker(Worker worker)
        {
            if (dbContext.Workers == null) return StatusCode(StatusCodes.Status500InternalServerError);

            dbContext.Entry(worker).State = EntityState.Modified;

            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WorkerExists(worker.Id))
                {
                    return NotFound();
                }
            }

            return NoContent();
        }

        private bool WorkerExists(int id)
        {
            if (dbContext.Workers == null) return false;

            return dbContext.Workers.Any(e => e.Id == id);
        }

        [HttpPost]
        public async Task<ActionResult<Worker>> PostWorker(Worker worker)
        {
            if (dbContext.Workers == null) return StatusCode(StatusCodes.Status500InternalServerError);

            dbContext.Workers.Add(worker);
            await dbContext.SaveChangesAsync();

            return CreatedAtAction("GetWorker", new { id = worker.Id }, worker);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWorker(int id)
        {
            if (dbContext.Workers == null) return StatusCode(StatusCodes.Status500InternalServerError);

            var worker = await dbContext.Workers.FindAsync(id);
            if (worker == null)
            {
                return NotFound();
            }

            dbContext.Workers.Remove(worker);
            await dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
