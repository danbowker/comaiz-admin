using Xunit;
using Microsoft.EntityFrameworkCore;
using comaiz.api.Controllers;
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
        public async Task GetContractsAsync_ReturnsAllContracts()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            context.Contracts!.Add(new Contract { Id = 1, Description = "Contract 1", ClientId = 1 });
            context.Contracts.Add(new Contract { Id = 2, Description = "Contract 2", ClientId = 1 });
            await context.SaveChangesAsync();
            
            var controller = new ContractsController(context);

            // Act
            var result = await controller.GetContractsAsync();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Contract>>>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<Contract>>(actionResult.Value);
            Assert.Equal(2, returnValue.Count());
        }

        [Fact]
        public async Task GetContractAsync_WithValidId_ReturnsContract()
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
        public async Task GetContractAsync_WithInvalidId_ReturnsNotFound()
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
        public async Task PostContract_WithValidContract_ReturnsCreatedAtAction()
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
        public async Task PutContract_WithValidContract_ReturnsNoContent()
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
        public async Task PutContract_WhenContractNotExists_ReturnsNotFound()
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
        public async Task DeleteContract_WithValidId_ReturnsNoContent()
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
        public async Task DeleteContract_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var controller = new ContractsController(context);

            // Act
            var result = await controller.DeleteContract(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
