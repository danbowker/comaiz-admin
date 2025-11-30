using Microsoft.AspNetCore.Mvc;
using comaiz.data;
using comaiz.data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;


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
        public async Task<ActionResult<IEnumerable<WorkRecord>>> GetWorkRecords([FromQuery] int? contractId)
        {
            if (dbContext.WorkRecords == null) return StatusCode(StatusCodes.Status500InternalServerError);

            var query = dbContext.WorkRecords
                .Include(wr => wr.Task)
                .AsQueryable();
            
            if (contractId.HasValue)
            {
                query = query.Where(wr => wr.Task != null && wr.Task.ContractId == contractId.Value);
            }

            return await query.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WorkRecord>> GetWorkRecord(int id)
        {
            if (dbContext.WorkRecords == null) return StatusCode(StatusCodes.Status500InternalServerError);

            var workRecord = await dbContext.WorkRecords.FindAsync(id);

            if (workRecord == null)
            {
                return NotFound();
            }

            return workRecord;
        }

        [HttpPut]
        public async Task<IActionResult> PutWorkRecord(WorkRecord workRecord)
        {
            if (dbContext.WorkRecords == null) return StatusCode(StatusCodes.Status500InternalServerError);

            dbContext.Entry(workRecord).State = EntityState.Modified;

            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WorkRecordExists(workRecord.Id))
                {
                    return NotFound();
                }
            }

            return NoContent();

        }

        private bool WorkRecordExists(int id)
        {
            if (dbContext.WorkRecords == null) return false;

            return dbContext.WorkRecords.Any(e => e.Id == id);
        }

        [HttpPost]
        public async Task<ActionResult<WorkRecord>> PostWorkRecord(WorkRecord workRecord)
        {
            if (dbContext.WorkRecords == null) return StatusCode(StatusCodes.Status500InternalServerError);

            // Validate that the task is not complete
            if (workRecord.TaskId.HasValue)
            {
                var task = await dbContext.Tasks!
                    .Include(t => t.Contract)
                    .FirstOrDefaultAsync(t => t.Id == workRecord.TaskId.Value);
                
                if (task != null)
                {
                    // Check if task itself is complete
                    if (task.State == RecordState.Complete)
                    {
                        return BadRequest("Cannot add work records to a complete task.");
                    }
                    
                    // Check if the contract is complete (tasks belong to complete contracts behave as complete)
                    if (task.Contract != null && task.Contract.State == RecordState.Complete)
                    {
                        return BadRequest("Cannot add work records to a task whose contract is complete.");
                    }
                }
            }

            dbContext.WorkRecords.Add(workRecord);
            await dbContext.SaveChangesAsync(); 

            return CreatedAtAction("GetWorkRecord", new { id = workRecord.Id }, workRecord);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWorkRecord(int id)
        {
            if (dbContext.WorkRecords == null) return StatusCode(StatusCodes.Status500InternalServerError);

            var workRecord = await dbContext.WorkRecords.FindAsync(id);
            if (workRecord == null)
            {
                return NotFound();
            }

            dbContext.WorkRecords.Remove(workRecord);
            await dbContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{id}/duplicate")]
        public async Task<ActionResult<WorkRecord>> DuplicateWorkRecord(int id)
        {
            if (dbContext.WorkRecords == null) return StatusCode(StatusCodes.Status500InternalServerError);

            var workRecord = await dbContext.WorkRecords.FindAsync(id);
            if (workRecord == null)
            {
                return NotFound();
            }

            var duplicatedWorkRecord = new WorkRecord
            {
                StartDate = workRecord.StartDate,
                EndDate = workRecord.EndDate,
                Hours = workRecord.Hours,
                ApplicationUserId = workRecord.ApplicationUserId,
                TaskId = workRecord.TaskId,
                InvoiceItemId = null // Don't copy the invoice item association
            };

            dbContext.WorkRecords.Add(duplicatedWorkRecord);
            await dbContext.SaveChangesAsync();

            return CreatedAtAction("GetWorkRecord", new { id = duplicatedWorkRecord.Id }, duplicatedWorkRecord);
        }
    }
}
