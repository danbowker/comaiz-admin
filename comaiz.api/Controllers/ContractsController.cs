using Microsoft.AspNetCore.Mvc;
using comaiz.data;
using comaiz.data.Models;
using comaiz.api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

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
        public async Task<ActionResult<IEnumerable<Contract>>> GetContractsAsync([FromQuery] RecordState? state)
        {
            if (dbContext.Contracts == null) return StatusCode(StatusCodes.Status500InternalServerError);

            var query = dbContext.Contracts.AsQueryable();
            
            if (state.HasValue)
            {
                query = query.Where(c => c.State == state.Value);
            }

            return await query.ToListAsync();
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

        [HttpPost("{id}/duplicate")]
        public async Task<ActionResult<Contract>> DuplicateContract(int id)
        {
            if (dbContext.Contracts == null) return StatusCode(StatusCodes.Status500InternalServerError);

            var contract = await dbContext.Contracts.FindAsync(id);
            if (contract == null)
            {
                return NotFound();
            }

            var duplicatedContract = new Contract
            {
                ClientId = contract.ClientId,
                Description = contract.Description != null ? $"{contract.Description} (Copy)" : null,
                Price = contract.Price,
                Schedule = contract.Schedule,
                ChargeType = contract.ChargeType
            };

            dbContext.Contracts.Add(duplicatedContract);
            await dbContext.SaveChangesAsync();

            return CreatedAtAction("GetContract", new { id = duplicatedContract.Id }, duplicatedContract);
        }

        [HttpGet("{id}/details")]
        public async Task<ActionResult<ContractDetailsDto>> GetContractDetailsAsync(int id)
        {
            if (dbContext.Contracts == null || dbContext.Tasks == null || dbContext.InvoiceItems == null || dbContext.Invoices == null)
                return StatusCode(StatusCodes.Status500InternalServerError);

            var contract = await dbContext.Contracts.FindAsync(id);
            if (contract == null)
            {
                return NotFound();
            }

            // Get all tasks for this contract
            var tasks = await dbContext.Tasks
                .Where(t => t.ContractId == id)
                .ToListAsync();

            var taskIds = tasks.Select(t => t.Id).ToList();

            // Get all invoice items for these tasks, including the invoice to check state
            var invoiceItems = await dbContext.InvoiceItems
                .Include(ii => ii.Invoice)
                .Where(ii => ii.TaskId != null && taskIds.Contains(ii.TaskId.Value))
                .ToListAsync();

            var taskDetails = new List<TaskDetailsDto>();

            foreach (var task in tasks)
            {
                var taskInvoiceItems = invoiceItems.Where(ii => ii.TaskId == task.Id).ToList();
                
                var taskTotalInvoiced = taskInvoiceItems
                    .Where(ii => ii.Invoice != null && ii.Invoice.State == InvoiceState.Issued)
                    .Sum(ii => ii.Price);

                var taskTotalPaid = taskInvoiceItems
                    .Where(ii => ii.Invoice != null && ii.Invoice.State == InvoiceState.Paid)
                    .Sum(ii => ii.Price);

                var taskLastEndDate = taskInvoiceItems
                    .Where(ii => ii.EndDate.HasValue)
                    .Select(ii => ii.EndDate!.Value)
                    .DefaultIfEmpty()
                    .Max();

                taskDetails.Add(new TaskDetailsDto
                {
                    TaskId = task.Id,
                    Name = task.Name,
                    TotalInvoiced = taskTotalInvoiced,
                    TotalPaid = taskTotalPaid,
                    LastInvoiceEndDate = taskLastEndDate == default ? null : taskLastEndDate
                });
            }

            // Calculate contract-level totals
            var contractTotalInvoiced = taskDetails.Sum(t => t.TotalInvoiced);
            var contractTotalPaid = taskDetails.Sum(t => t.TotalPaid);
            
            // For the contract's last invoice end date, get the earliest of all task last end dates
            // (per the requirement: "For a contract, this is the earliest of all 'Last invoice end dates' for the tasks")
            var taskLastEndDates = taskDetails
                .Where(t => t.LastInvoiceEndDate.HasValue)
                .Select(t => t.LastInvoiceEndDate!.Value)
                .ToList();
            
            DateOnly? contractLastEndDate = taskLastEndDates.Any() ? taskLastEndDates.Min() : null;

            // Calculate remaining (only if contract has a price)
            decimal? remaining = contract.Price.HasValue 
                ? contract.Price.Value - contractTotalPaid 
                : null;

            return new ContractDetailsDto
            {
                ContractId = contract.Id,
                Description = contract.Description,
                Price = contract.Price,
                TotalInvoiced = contractTotalInvoiced,
                TotalPaid = contractTotalPaid,
                Remaining = remaining,
                LastInvoiceEndDate = contractLastEndDate,
                Tasks = taskDetails
            };
        }
    }
}
