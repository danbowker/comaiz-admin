using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using comaiz.data;
using comaiz.data.Models;
using Microsoft.EntityFrameworkCore;


namespace comaiz.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkRecordsController : ControllerBase
    {
        private readonly ComaizContext dbContext;
        public WorkRecordsController(ComaizContext context)
        {
            dbContext = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WorkRecord>>> GetWorkRecords()
        {
            return await dbContext.WorkRecords.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WorkRecord>> GetWorkRecord(int id)
        {
            var workRecord = await dbContext.WorkRecords.FindAsync(id);

            if (workRecord == null)
            {
                return NotFound();
            }

            return workRecord;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutWorkRecord(int id, WorkRecord workRecord)
        {
            if (id != workRecord.Id)
            {
                return BadRequest();
            }

            dbContext.Entry(workRecord).State = EntityState.Modified;

            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WorkRecordExists(id))
                {
                    return NotFound();
                }
            }

            return NoContent();

        }

        private bool WorkRecordExists(int id)
        {
            return dbContext.WorkRecords.Any(e => e.Id == id);
        }

        [HttpPost]
        public async Task<ActionResult<WorkRecord>> PostWorkRecord(WorkRecord workRecord)
        {
            dbContext.WorkRecords.Add(workRecord);
            await dbContext.SaveChangesAsync();

            return CreatedAtAction("GetWorkRecord", new { id = workRecord.Id }, workRecord);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWorkRecord(int id)
        {
            var workRecord = await dbContext.WorkRecords.FindAsync(id);
            if (workRecord == null)
            {
                return NotFound();
            }

            dbContext.WorkRecords.Remove(workRecord);
            await dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
