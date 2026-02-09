using Microsoft.AspNetCore.Mvc;
using comaiz.data;
using comaiz.data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using comaiz.api.Models;


namespace comaiz.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceItemsController : ControllerBase
    {
        private readonly ComaizContext dbContext;

        public InvoiceItemsController(ComaizContext context)
        {
            dbContext = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InvoiceItem>>> GetInvoiceItems([FromQuery] int? contractId)
        {
            if (dbContext.InvoiceItems == null) return StatusCode(StatusCodes.Status500InternalServerError);

            var query = dbContext.InvoiceItems
                .Include(ii => ii.Task)
                .Include(ii => ii.FixedCost)
                .AsQueryable();
            
            if (contractId.HasValue)
            {
                query = query.Where(ii => 
                    (ii.Task != null && ii.Task.ContractId == contractId.Value) ||
                    (ii.FixedCost != null && ii.FixedCost.ContractId == contractId.Value)
                );
            }

            return await query.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InvoiceItem>> GetInvoiceItem(int id)
        {
            if (dbContext.InvoiceItems == null) return StatusCode(StatusCodes.Status500InternalServerError);

            var invoiceItem = await dbContext.InvoiceItems.FindAsync(id);

            if (invoiceItem == null)
            {
                return NotFound();
            }

            return invoiceItem;
        }

        [HttpPut]
        public async Task<IActionResult> PutInvoiceItem(InvoiceItem invoiceItem)
        {
            if (dbContext.InvoiceItems == null) return StatusCode(StatusCodes.Status500InternalServerError);

            dbContext.Entry(invoiceItem).State = EntityState.Modified;

            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InvoiceItemExists(invoiceItem.Id))
                {
                    return NotFound();
                }
            }

            return NoContent();
        }

        private bool InvoiceItemExists(int id)
        {
            if (dbContext.InvoiceItems == null) return false;

            return dbContext.InvoiceItems.Any(e => e.Id == id);
        }

        [HttpPost]
        public async Task<ActionResult<InvoiceItem>> PostInvoiceItem(InvoiceItem invoiceItem)
        {
            if(dbContext.InvoiceItems == null) return StatusCode(StatusCodes.Status500InternalServerError);

            dbContext.InvoiceItems.Add(invoiceItem);
            await dbContext.SaveChangesAsync();

            return CreatedAtAction("GetInvoiceItem", new { id = invoiceItem.Id }, invoiceItem);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoiceItem(int id)
        {
            if (dbContext.InvoiceItems == null) return StatusCode(StatusCodes.Status500InternalServerError);

            var invoiceItem = await dbContext.InvoiceItems.FindAsync(id);
            if (invoiceItem == null)
            {
                return NotFound();
            }

            dbContext.InvoiceItems.Remove(invoiceItem);
            await dbContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{id}/duplicate")]
        public async Task<ActionResult<InvoiceItem>> DuplicateInvoiceItem(int id)
        {
            if (dbContext.InvoiceItems == null) return StatusCode(StatusCodes.Status500InternalServerError);

            var invoiceItem = await dbContext.InvoiceItems.FindAsync(id);
            if (invoiceItem == null)
            {
                return NotFound();
            }

            var duplicatedInvoiceItem = new InvoiceItem
            {
                InvoiceId = invoiceItem.InvoiceId,
                TaskId = invoiceItem.TaskId,
                FixedCostId = invoiceItem.FixedCostId,
                Quantity = invoiceItem.Quantity,
                Unit = invoiceItem.Unit,
                Rate = invoiceItem.Rate,
                VATRate = invoiceItem.VATRate,
                Price = invoiceItem.Price,
                PriceIncVAT = invoiceItem.PriceIncVAT,
                Description = invoiceItem.Description,
                StartDate = invoiceItem.StartDate,
                EndDate = invoiceItem.EndDate
            };

            dbContext.InvoiceItems.Add(duplicatedInvoiceItem);
            await dbContext.SaveChangesAsync();

            return CreatedAtAction("GetInvoiceItem", new { id = duplicatedInvoiceItem.Id }, duplicatedInvoiceItem);
        }

        [HttpPost("create-fixed-cost")]
        public async Task<ActionResult<InvoiceItem>> CreateFixedCostInvoiceItem(CreateFixedCostInvoiceItemDto dto)
        {
            if (dbContext.InvoiceItems == null || dbContext.FixedCosts == null) 
                return StatusCode(StatusCodes.Status500InternalServerError);

            // Get the fixed cost to retrieve its amount
            var fixedCost = await dbContext.FixedCosts.FindAsync(dto.FixedCostId);
            if (fixedCost == null)
            {
                return BadRequest("Fixed cost not found");
            }

            var rate = fixedCost.Amount ?? 0m;
            var quantity = 1m;
            var price = quantity * rate;
            var priceIncVAT = price * (1 + dto.VATRate);

            var invoiceItem = new InvoiceItem
            {
                InvoiceId = dto.InvoiceId,
                FixedCostId = dto.FixedCostId,
                Quantity = quantity,
                Unit = Unit.Hours, // Default unit
                Rate = rate,
                VATRate = dto.VATRate,
                Price = price,
                PriceIncVAT = priceIncVAT,
                Description = fixedCost.Name
            };

            dbContext.InvoiceItems.Add(invoiceItem);
            await dbContext.SaveChangesAsync();

            return CreatedAtAction("GetInvoiceItem", new { id = invoiceItem.Id }, invoiceItem);
        }

        [HttpPost("create-labour-cost")]
        public async Task<ActionResult<InvoiceItem>> CreateLabourCostInvoiceItem(CreateLabourCostInvoiceItemDto dto)
        {
            if (dbContext.InvoiceItems == null || dbContext.Tasks == null || dbContext.WorkRecords == null) 
                return StatusCode(StatusCodes.Status500InternalServerError);

            // Get the task with related data
            var task = await dbContext.Tasks
                .Include(t => t.TaskContractRates)
                    .ThenInclude(tcr => tcr.UserContractRate)
                        .ThenInclude(ucr => ucr!.ContractRate)
                .Include(t => t.TaskContractRates)
                    .ThenInclude(tcr => tcr.UserContractRate)
                        .ThenInclude(ucr => ucr!.ApplicationUser)
                .FirstOrDefaultAsync(t => t.Id == dto.TaskId);
            
            if (task == null)
            {
                return BadRequest("Task not found");
            }

            // Calculate quantity from work records if date range is provided
            decimal quantity = await CalculateQuantityFromWorkRecords(dto);
            
            // If quantity was provided manually, use that instead
            if (dto.Quantity.HasValue)
            {
                quantity = dto.Quantity.Value;
            }

            // Get rate from user contract rate for the task
            decimal rate = GetRateFromTaskContractRate(task, dto.ApplicationUserId);
            
            // If rate was provided manually, use that instead
            if (dto.Rate.HasValue)
            {
                rate = dto.Rate.Value;
            }

            // Get worker name and generate description if not provided
            string? workerName = null;
            if (!string.IsNullOrEmpty(dto.ApplicationUserId))
            {
                var user = await dbContext.Users.FindAsync(dto.ApplicationUserId);
                workerName = user?.UserName;
            }

            string description = GenerateLabourDescription(dto.Description, task.Name, workerName, dto.StartDate, dto.EndDate);

            var price = quantity * rate;
            var priceIncVAT = price * (1 + dto.VATRate);

            var invoiceItem = new InvoiceItem
            {
                InvoiceId = dto.InvoiceId,
                TaskId = dto.TaskId,
                Quantity = quantity,
                Unit = Unit.Hours,
                Rate = rate,
                VATRate = dto.VATRate,
                Price = price,
                PriceIncVAT = priceIncVAT,
                Description = description,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate
            };

            dbContext.InvoiceItems.Add(invoiceItem);
            await dbContext.SaveChangesAsync();

            return CreatedAtAction("GetInvoiceItem", new { id = invoiceItem.Id }, invoiceItem);
        }

        private async Task<decimal> CalculateQuantityFromWorkRecords(CreateLabourCostInvoiceItemDto dto)
        {
            if (dto.StartDate.HasValue && dto.EndDate.HasValue && !string.IsNullOrEmpty(dto.ApplicationUserId))
            {
                var workRecords = await dbContext.WorkRecords!
                    .Where(wr => wr.TaskId == dto.TaskId 
                        && wr.ApplicationUserId == dto.ApplicationUserId
                        && wr.StartDate >= dto.StartDate.Value
                        && wr.EndDate <= dto.EndDate.Value)
                    .ToListAsync();
                
                return workRecords.Sum(wr => wr.Hours);
            }
            return 0m;
        }

        private decimal GetRateFromTaskContractRate(comaiz.data.Models.Task task, string? applicationUserId)
        {
            if (!string.IsNullOrEmpty(applicationUserId))
            {
                var taskContractRate = task.TaskContractRates?
                    .FirstOrDefault(tcr => tcr.UserContractRate?.ApplicationUserId == applicationUserId);
                
                if (taskContractRate?.UserContractRate?.ContractRate?.Rate.HasValue == true)
                {
                    return taskContractRate.UserContractRate.ContractRate.Rate.Value;
                }
            }
            return 0m;
        }

        private string GenerateLabourDescription(string? providedDescription, string taskName, string? workerName, DateOnly? startDate, DateOnly? endDate)
        {
            if (!string.IsNullOrEmpty(providedDescription))
            {
                return providedDescription;
            }

            var parts = new List<string> { taskName };
            
            if (!string.IsNullOrEmpty(workerName))
            {
                parts.Add(workerName);
            }
            
            if (startDate.HasValue && endDate.HasValue)
            {
                parts.Add($"{startDate.Value:yyyy-MM-dd} to {endDate.Value:yyyy-MM-dd}");
            }
            
            return string.Join(" - ", parts);
        }

        [HttpPost("create-mileage-cost")]
        public async Task<ActionResult<InvoiceItem>> CreateMileageCostInvoiceItem(CreateMileageCostInvoiceItemDto dto)
        {
            if (dbContext.InvoiceItems == null) 
                return StatusCode(StatusCodes.Status500InternalServerError);

            var price = dto.Quantity * dto.Rate;
            var priceIncVAT = price * (1 + dto.VATRate);

            var invoiceItem = new InvoiceItem
            {
                InvoiceId = dto.InvoiceId,
                Quantity = dto.Quantity,
                Unit = Unit.Miles,
                Rate = dto.Rate,
                VATRate = dto.VATRate,
                Price = price,
                PriceIncVAT = priceIncVAT,
                Description = dto.Description ?? "Mileage"
            };

            dbContext.InvoiceItems.Add(invoiceItem);
            await dbContext.SaveChangesAsync();

            return CreatedAtAction("GetInvoiceItem", new { id = invoiceItem.Id }, invoiceItem);
        }
    }
}
