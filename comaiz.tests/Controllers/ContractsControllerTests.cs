using Xunit;
using Microsoft.EntityFrameworkCore;
using comaiz.api.Controllers;
using comaiz.api.Models;
using comaiz.data;
using comaiz.data.Models;
using Microsoft.AspNetCore.Mvc;

namespace comaiz.tests.Controllers
{
    /// <summary>
    /// Unit tests for ContractsController using In-Memory Database.
    /// Tests all CRUD operations with success and failure scenarios.
    /// </summary>
    public class ContractsControllerTests
    {
        private ComaizContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ComaizContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            return new ComaizContext(options);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetContractsAsync_ReturnsAllContracts()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            context.Contracts!.Add(new Contract { Id = 1, Description = "Contract 1", ClientId = 1 });
            context.Contracts.Add(new Contract { Id = 2, Description = "Contract 2", ClientId = 1 });
            await context.SaveChangesAsync();
            
            var controller = new ContractsController(context);

            // Act
            var result = await controller.GetContractsAsync(null);

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Contract>>>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<Contract>>(actionResult.Value);
            Assert.Equal(2, returnValue.Count());
        }

        [Fact]
        public async System.Threading.Tasks.Task GetContractAsync_WithValidId_ReturnsContract()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            context.Contracts!.Add(new Contract { Id = 1, Description = "Contract 1", ClientId = 1 });
            await context.SaveChangesAsync();
            
            var controller = new ContractsController(context);

            // Act
            var result = await controller.GetContractAsync(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Contract>>(result);
            var returnValue = Assert.IsType<Contract>(actionResult.Value);
            Assert.Equal(1, returnValue.Id);
            Assert.Equal("Contract 1", returnValue.Description);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetContractAsync_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var controller = new ContractsController(context);

            // Act
            var result = await controller.GetContractAsync(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async System.Threading.Tasks.Task PostContract_WithValidContract_ReturnsCreatedAtAction()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var contract = new Contract { Description = "New Contract", ClientId = 1 };
            var controller = new ContractsController(context);

            // Act
            var result = await controller.PostContract(contract);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Contract>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            Assert.Equal("GetContract", createdAtActionResult.ActionName);
            
            // Verify it was actually saved
            var savedContract = await context.Contracts!.FindAsync(contract.Id);
            Assert.NotNull(savedContract);
            Assert.Equal("New Contract", savedContract.Description);
        }

        [Fact]
        public async System.Threading.Tasks.Task PostContract_WithPlannedDates_SavesDatesCorrectly()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var plannedStart = new DateOnly(2025, 1, 1);
            var plannedEnd = new DateOnly(2025, 12, 31);
            var contract = new Contract 
            { 
                Description = "Contract with Dates", 
                ClientId = 1,
                PlannedStart = plannedStart,
                PlannedEnd = plannedEnd
            };
            var controller = new ContractsController(context);

            // Act
            var result = await controller.PostContract(contract);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Contract>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            
            // Verify dates were saved correctly
            var savedContract = await context.Contracts!.FindAsync(contract.Id);
            Assert.NotNull(savedContract);
            Assert.Equal(plannedStart, savedContract.PlannedStart);
            Assert.Equal(plannedEnd, savedContract.PlannedEnd);
        }

        [Fact]
        public async System.Threading.Tasks.Task PutContract_WithValidContract_ReturnsNoContent()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var contract = new Contract { Id = 1, Description = "Original", ClientId = 1 };
            context.Contracts!.Add(contract);
            await context.SaveChangesAsync();
            
            context.Entry(contract).State = EntityState.Detached;
            
            var updatedContract = new Contract { Id = 1, Description = "Updated", ClientId = 1 };
            var controller = new ContractsController(context);

            // Act
            var result = await controller.PutContract(updatedContract);

            // Assert
            Assert.IsType<NoContentResult>(result);
            
            var savedContract = await context.Contracts.FindAsync(1);
            Assert.Equal("Updated", savedContract!.Description);
        }

        [Fact]
        public async System.Threading.Tasks.Task PutContract_WhenContractNotExists_ReturnsNotFound()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var contract = new Contract { Id = 999, Description = "Contract", ClientId = 1 };
            var controller = new ContractsController(context);

            // Act
            var result = await controller.PutContract(contract);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async System.Threading.Tasks.Task PutContract_UpdatesPlannedDates()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var contract = new Contract { Id = 1, Description = "Original", ClientId = 1 };
            context.Contracts!.Add(contract);
            await context.SaveChangesAsync();
            
            context.Entry(contract).State = EntityState.Detached;
            
            var plannedStart = new DateOnly(2025, 6, 1);
            var plannedEnd = new DateOnly(2025, 12, 31);
            var updatedContract = new Contract 
            { 
                Id = 1, 
                Description = "Updated", 
                ClientId = 1,
                PlannedStart = plannedStart,
                PlannedEnd = plannedEnd
            };
            var controller = new ContractsController(context);

            // Act
            var result = await controller.PutContract(updatedContract);

            // Assert
            Assert.IsType<NoContentResult>(result);
            
            var savedContract = await context.Contracts.FindAsync(1);
            Assert.Equal("Updated", savedContract!.Description);
            Assert.Equal(plannedStart, savedContract.PlannedStart);
            Assert.Equal(plannedEnd, savedContract.PlannedEnd);
        }

        [Fact]
        public async System.Threading.Tasks.Task DeleteContract_WithValidId_ReturnsNoContent()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var contract = new Contract { Id = 1, Description = "Contract 1", ClientId = 1 };
            context.Contracts!.Add(contract);
            await context.SaveChangesAsync();
            
            var controller = new ContractsController(context);

            // Act
            var result = await controller.DeleteContract(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            
            var deletedContract = await context.Contracts.FindAsync(1);
            Assert.Null(deletedContract);
        }

        [Fact]
        public async System.Threading.Tasks.Task DeleteContract_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var controller = new ContractsController(context);

            // Act
            var result = await controller.DeleteContract(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async System.Threading.Tasks.Task DuplicateContract_WithValidId_ReturnsCreatedAtAction()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var contract = new Contract
            {
                Id = 1,
                Description = "Test Contract",
                ClientId = 1,
                Price = 1000.00m,
                Schedule = "Monthly",
                ChargeType = ChargeType.TimeAndMaterials
            };
            context.Contracts!.Add(contract);
            await context.SaveChangesAsync();

            var controller = new ContractsController(context);

            // Act
            var result = await controller.DuplicateContract(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Contract>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            Assert.Equal("GetContract", createdAtActionResult.ActionName);

            var duplicatedContract = Assert.IsType<Contract>(createdAtActionResult.Value);
            Assert.NotEqual(1, duplicatedContract.Id);
            Assert.Equal("Test Contract (Copy)", duplicatedContract.Description);
            Assert.Equal(1, duplicatedContract.ClientId);
            Assert.Equal(1000.00m, duplicatedContract.Price);
            Assert.Equal("Monthly", duplicatedContract.Schedule);
            Assert.Equal(ChargeType.TimeAndMaterials, duplicatedContract.ChargeType);

            // Verify both contracts exist in the database
            var allContracts = await context.Contracts.ToListAsync();
            Assert.Equal(2, allContracts.Count);
        }

        [Fact]
        public async System.Threading.Tasks.Task DuplicateContract_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var controller = new ContractsController(context);

            // Act
            var result = await controller.DuplicateContract(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetContractsAsync_WithStateFilter_ReturnsFilteredContracts()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            context.Contracts!.Add(new Contract { Id = 1, Description = "Active Contract", ClientId = 1, State = RecordState.Active });
            context.Contracts.Add(new Contract { Id = 2, Description = "Complete Contract", ClientId = 1, State = RecordState.Complete });
            await context.SaveChangesAsync();
            
            var controller = new ContractsController(context);

            // Act - Filter for active contracts
            var resultActive = await controller.GetContractsAsync(RecordState.Active);
            var activeContracts = Assert.IsAssignableFrom<IEnumerable<Contract>>(resultActive.Value);
            
            // Act - Filter for complete contracts
            var resultComplete = await controller.GetContractsAsync(RecordState.Complete);
            var completeContracts = Assert.IsAssignableFrom<IEnumerable<Contract>>(resultComplete.Value);

            // Assert
            Assert.Single(activeContracts);
            Assert.Equal(1, activeContracts.First().Id);
            Assert.Single(completeContracts);
            Assert.Equal(2, completeContracts.First().Id);
        }

        [Fact]
        public async System.Threading.Tasks.Task PostContract_DefaultStateIsActive()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var contract = new Contract { Description = "New Contract", ClientId = 1 };
            var controller = new ContractsController(context);

            // Act
            var result = await controller.PostContract(contract);

            // Assert
            var savedContract = await context.Contracts!.FindAsync(contract.Id);
            Assert.NotNull(savedContract);
            Assert.Equal(RecordState.Active, savedContract.State);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetContractDetailsAsync_WithValidId_ReturnsDetails()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var contract = new Contract { Id = 1, Description = "Test Contract", ClientId = 1, Price = 10000m };
            context.Contracts!.Add(contract);
            
            var task1 = new comaiz.data.Models.Task { Id = 1, Name = "Task 1", ContractId = 1 };
            var task2 = new comaiz.data.Models.Task { Id = 2, Name = "Task 2", ContractId = 1 };
            context.Tasks!.AddRange(task1, task2);
            
            var invoice1 = new Invoice { Id = 1, Date = DateOnly.FromDateTime(DateTime.Now), ClientId = 1, State = InvoiceState.Issued };
            var invoice2 = new Invoice { Id = 2, Date = DateOnly.FromDateTime(DateTime.Now), ClientId = 1, State = InvoiceState.Paid };
            context.Invoices!.AddRange(invoice1, invoice2);
            
            // Invoice items for task 1
            context.InvoiceItems!.Add(new InvoiceItem 
            { 
                Id = 1, InvoiceId = 1, TaskId = 1, Quantity = 10, Rate = 100m, Price = 1000m,
                StartDate = new DateOnly(2025, 1, 1), EndDate = new DateOnly(2025, 1, 31)
            });
            context.InvoiceItems.Add(new InvoiceItem 
            { 
                Id = 2, InvoiceId = 2, TaskId = 1, Quantity = 5, Rate = 100m, Price = 500m,
                StartDate = new DateOnly(2025, 2, 1), EndDate = new DateOnly(2025, 2, 28)
            });
            
            // Invoice items for task 2
            context.InvoiceItems.Add(new InvoiceItem 
            { 
                Id = 3, InvoiceId = 1, TaskId = 2, Quantity = 20, Rate = 50m, Price = 1000m,
                StartDate = new DateOnly(2025, 1, 15), EndDate = new DateOnly(2025, 2, 15)
            });
            
            await context.SaveChangesAsync();
            
            var controller = new ContractsController(context);

            // Act
            var result = await controller.GetContractDetailsAsync(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ContractDetailsDto>>(result);
            var details = Assert.IsType<ContractDetailsDto>(actionResult.Value);
            
            Assert.Equal(1, details.ContractId);
            Assert.Equal("Test Contract", details.Description);
            Assert.Equal(10000m, details.Price);
            Assert.Equal(2000m, details.TotalInvoiced); // 1000 + 1000 from Issued invoices
            Assert.Equal(500m, details.TotalPaid); // 500 from Paid invoices
            Assert.Equal(9500m, details.Remaining); // 10000 - 500
            Assert.Equal(2, details.Tasks.Count);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetContractDetailsAsync_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var controller = new ContractsController(context);

            // Act
            var result = await controller.GetContractDetailsAsync(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetContractDetailsAsync_ContractWithNoPrice_RemainingIsNull()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var contract = new Contract { Id = 1, Description = "T&M Contract", ClientId = 1, Price = null };
            context.Contracts!.Add(contract);
            await context.SaveChangesAsync();
            
            var controller = new ContractsController(context);

            // Act
            var result = await controller.GetContractDetailsAsync(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ContractDetailsDto>>(result);
            var details = Assert.IsType<ContractDetailsDto>(actionResult.Value);
            
            Assert.Null(details.Price);
            Assert.Null(details.Remaining);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetContractDetailsAsync_LastInvoiceEndDateIsEarliestOfTaskEndDates()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var contract = new Contract { Id = 1, Description = "Test Contract", ClientId = 1 };
            context.Contracts!.Add(contract);
            
            var task1 = new comaiz.data.Models.Task { Id = 1, Name = "Task 1", ContractId = 1 };
            var task2 = new comaiz.data.Models.Task { Id = 2, Name = "Task 2", ContractId = 1 };
            context.Tasks!.AddRange(task1, task2);
            
            var invoice = new Invoice { Id = 1, Date = DateOnly.FromDateTime(DateTime.Now), ClientId = 1, State = InvoiceState.Issued };
            context.Invoices!.Add(invoice);
            
            // Task 1 has end date 2025-01-31
            context.InvoiceItems!.Add(new InvoiceItem 
            { 
                Id = 1, InvoiceId = 1, TaskId = 1, Quantity = 10, Rate = 100m, Price = 1000m,
                EndDate = new DateOnly(2025, 1, 31)
            });
            
            // Task 2 has end date 2025-02-15 (later)
            context.InvoiceItems.Add(new InvoiceItem 
            { 
                Id = 2, InvoiceId = 1, TaskId = 2, Quantity = 20, Rate = 50m, Price = 1000m,
                EndDate = new DateOnly(2025, 2, 15)
            });
            
            await context.SaveChangesAsync();
            
            var controller = new ContractsController(context);

            // Act
            var result = await controller.GetContractDetailsAsync(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ContractDetailsDto>>(result);
            var details = Assert.IsType<ContractDetailsDto>(actionResult.Value);
            
            // Contract's last invoice end date should be the earliest of task end dates
            Assert.Equal(new DateOnly(2025, 1, 31), details.LastInvoiceEndDate);
            
            // Task 1's last end date
            var task1Details = details.Tasks.First(t => t.TaskId == 1);
            Assert.Equal(new DateOnly(2025, 1, 31), task1Details.LastInvoiceEndDate);
            
            // Task 2's last end date
            var task2Details = details.Tasks.First(t => t.TaskId == 2);
            Assert.Equal(new DateOnly(2025, 2, 15), task2Details.LastInvoiceEndDate);
        }
    }
}
