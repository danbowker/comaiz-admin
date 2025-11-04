using Xunit;
using Microsoft.EntityFrameworkCore;
using comaiz.api.Controllers;
using comaiz.data;
using comaiz.data.Models;
using Microsoft.AspNetCore.Mvc;

namespace comaiz.tests.Controllers
{
    /// <summary>
    /// Unit tests for FixedCostsController using In-Memory Database.
    /// Tests all CRUD operations with success and failure scenarios.
    /// </summary>
    public class FixedCostsControllerTests
    {
        private ComaizContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ComaizContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            return new ComaizContext(options);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetCosts_ReturnsAllFixedCosts()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            context.FixedCosts!.Add(new FixedCost { Id = 1, Name = "Cost 1", Amount = 100m, ContractId = 1 });
            context.FixedCosts.Add(new FixedCost { Id = 2, Name = "Cost 2", Amount = 200m, ContractId = 1 });
            await context.SaveChangesAsync();
            
            var controller = new FixedCostsController(context);

            // Act
            var result = await controller.GetCosts(null);

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Cost>>>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<Cost>>(actionResult.Value);
            Assert.Equal(2, returnValue.Count());
        }

        [Fact]
        public async System.Threading.Tasks.Task GetCost_WithValidId_ReturnsFixedCost()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            context.FixedCosts!.Add(new FixedCost { Id = 1, Name = "Cost 1", Amount = 100m, ContractId = 1 });
            await context.SaveChangesAsync();
            
            var controller = new FixedCostsController(context);

            // Act
            var result = await controller.GetCost(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Cost>>(result);
            var returnValue = Assert.IsType<FixedCost>(actionResult.Value);
            Assert.Equal(1, returnValue.Id);
            Assert.Equal("Cost 1", returnValue.Name);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetCost_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var controller = new FixedCostsController(context);

            // Act
            var result = await controller.GetCost(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async System.Threading.Tasks.Task PostCost_WithValidFixedCost_ReturnsCreatedAtAction()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var cost = new FixedCost { Name = "New Cost", Amount = 150m, ContractId = 1 };
            var controller = new FixedCostsController(context);

            // Act
            var result = await controller.PostCost(cost);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Cost>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            Assert.Equal("GetCost", createdAtActionResult.ActionName);
            
            var savedCost = await context.FixedCosts!.FindAsync(cost.Id);
            Assert.NotNull(savedCost);
            Assert.Equal("New Cost", savedCost.Name);
        }

        [Fact]
        public async System.Threading.Tasks.Task PutCost_WithValidFixedCost_ReturnsNoContent()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var cost = new FixedCost { Id = 1, Name = "Original", Amount = 100m, ContractId = 1 };
            context.FixedCosts!.Add(cost);
            await context.SaveChangesAsync();
            
            context.Entry(cost).State = EntityState.Detached;
            
            var updatedCost = new FixedCost { Id = 1, Name = "Updated", Amount = 200m, ContractId = 1 };
            var controller = new FixedCostsController(context);

            // Act
            var result = await controller.PutCost(updatedCost);

            // Assert
            Assert.IsType<NoContentResult>(result);
            
            var savedCost = await context.FixedCosts.FindAsync(1);
            Assert.Equal("Updated", savedCost!.Name);
            Assert.Equal(200m, savedCost.Amount);
        }

        [Fact]
        public async System.Threading.Tasks.Task PutCost_WhenFixedCostNotExists_ReturnsNotFound()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var cost = new FixedCost { Id = 999, Name = "Cost", Amount = 100m, ContractId = 1 };
            var controller = new FixedCostsController(context);

            // Act
            var result = await controller.PutCost(cost);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async System.Threading.Tasks.Task DeleteCost_WithValidId_ReturnsNoContent()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var cost = new FixedCost { Id = 1, Name = "Cost 1", Amount = 100m, ContractId = 1 };
            context.FixedCosts!.Add(cost);
            await context.SaveChangesAsync();
            
            var controller = new FixedCostsController(context);

            // Act
            var result = await controller.DeleteCost(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            
            var deletedCost = await context.FixedCosts.FindAsync(1);
            Assert.Null(deletedCost);
        }

        [Fact]
        public async System.Threading.Tasks.Task DeleteCost_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var controller = new FixedCostsController(context);

            // Act
            var result = await controller.DeleteCost(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
