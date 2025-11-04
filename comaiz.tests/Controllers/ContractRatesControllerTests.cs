using Xunit;
using Microsoft.EntityFrameworkCore;
using comaiz.api.Controllers;
using comaiz.data;
using comaiz.data.Models;
using Microsoft.AspNetCore.Mvc;

namespace comaiz.tests.Controllers
{
    /// <summary>
    /// Unit tests for ContractRatesController using In-Memory Database.
    /// Tests all CRUD operations with success and failure scenarios.
    /// </summary>
    public class ContractRatesControllerTests
    {
        private ComaizContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ComaizContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            return new ComaizContext(options);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetContractRates_ReturnsAllContractRates()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            context.ContractRates!.Add(new ContractRate { Id = 1, Description = "Rate 1", Rate = 100m, ContractId = 1 });
            context.ContractRates.Add(new ContractRate { Id = 2, Description = "Rate 2", Rate = 150m, ContractId = 1 });
            await context.SaveChangesAsync();
            
            var controller = new ContractRatesController(context);

            // Act
            var result = await controller.GetContractRates(null);

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<ContractRate>>>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<ContractRate>>(actionResult.Value);
            Assert.Equal(2, returnValue.Count());
        }

        [Fact]
        public async System.Threading.Tasks.Task GetContractRate_WithValidId_ReturnsContractRate()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            context.ContractRates!.Add(new ContractRate { Id = 1, Description = "Rate 1", Rate = 100m, ContractId = 1 });
            await context.SaveChangesAsync();
            
            var controller = new ContractRatesController(context);

            // Act
            var result = await controller.GetContractRate(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ContractRate>>(result);
            var returnValue = Assert.IsType<ContractRate>(actionResult.Value);
            Assert.Equal(1, returnValue.Id);
            Assert.Equal("Rate 1", returnValue.Description);
            Assert.Equal(100m, returnValue.Rate);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetContractRate_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var controller = new ContractRatesController(context);

            // Act
            var result = await controller.GetContractRate(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async System.Threading.Tasks.Task PostContractRate_WithValidContractRate_ReturnsCreatedAtAction()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var contractRate = new ContractRate { Description = "New Rate", Rate = 125m, ContractId = 1 };
            var controller = new ContractRatesController(context);

            // Act
            var result = await controller.PostContractRate(contractRate);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ContractRate>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            Assert.Equal("GetContractRate", createdAtActionResult.ActionName);
            
            var savedContractRate = await context.ContractRates!.FindAsync(contractRate.Id);
            Assert.NotNull(savedContractRate);
            Assert.Equal("New Rate", savedContractRate.Description);
            Assert.Equal(125m, savedContractRate.Rate);
        }

        [Fact]
        public async System.Threading.Tasks.Task PutContractRate_WithValidContractRate_ReturnsNoContent()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var contractRate = new ContractRate { Id = 1, Description = "Original", Rate = 100m, ContractId = 1 };
            context.ContractRates!.Add(contractRate);
            await context.SaveChangesAsync();
            
            context.Entry(contractRate).State = EntityState.Detached;
            
            var updatedContractRate = new ContractRate { Id = 1, Description = "Updated", Rate = 200m, ContractId = 1 };
            var controller = new ContractRatesController(context);

            // Act
            var result = await controller.PutContractRate(updatedContractRate);

            // Assert
            Assert.IsType<NoContentResult>(result);
            
            var savedContractRate = await context.ContractRates.FindAsync(1);
            Assert.Equal("Updated", savedContractRate!.Description);
            Assert.Equal(200m, savedContractRate.Rate);
        }

        [Fact]
        public async System.Threading.Tasks.Task PutContractRate_WhenContractRateNotExists_ReturnsNotFound()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var contractRate = new ContractRate { Id = 999, Description = "Rate", Rate = 100m, ContractId = 1 };
            var controller = new ContractRatesController(context);

            // Act
            var result = await controller.PutContractRate(contractRate);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async System.Threading.Tasks.Task DeleteContractRate_WithValidId_ReturnsNoContent()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var contractRate = new ContractRate { Id = 1, Description = "Rate 1", Rate = 100m, ContractId = 1 };
            context.ContractRates!.Add(contractRate);
            await context.SaveChangesAsync();
            
            var controller = new ContractRatesController(context);

            // Act
            var result = await controller.DeleteContractRate(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            
            var deletedContractRate = await context.ContractRates.FindAsync(1);
            Assert.Null(deletedContractRate);
        }

        [Fact]
        public async System.Threading.Tasks.Task DeleteContractRate_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var controller = new ContractRatesController(context);

            // Act
            var result = await controller.DeleteContractRate(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async System.Threading.Tasks.Task PostContractRate_WithUser_ReturnsCreatedAtActionWithUser()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var user = new ApplicationUser { Id = "test-user-id", UserName = "testuser", Email = "test@example.com" };
            context.Users!.Add(user);
            await context.SaveChangesAsync();
            
            var contractRate = new ContractRate { Description = "Rate with User", Rate = 150m, ContractId = 1, ApplicationUserId = user.Id };
            var controller = new ContractRatesController(context);

            // Act
            var result = await controller.PostContractRate(contractRate);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ContractRate>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            Assert.Equal("GetContractRate", createdAtActionResult.ActionName);
            
            var savedContractRate = await context.ContractRates!.FindAsync(contractRate.Id);
            Assert.NotNull(savedContractRate);
            Assert.Equal("Rate with User", savedContractRate.Description);
            Assert.Equal(150m, savedContractRate.Rate);
            Assert.Equal(user.Id, savedContractRate.ApplicationUserId);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetContractRate_WithUser_IncludesUserData()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var user = new ApplicationUser { Id = "test-user-id-2", UserName = "testuser2", Email = "test2@example.com" };
            context.Users!.Add(user);
            await context.SaveChangesAsync();
            
            context.ContractRates!.Add(new ContractRate { Id = 1, Description = "Rate 1", Rate = 100m, ContractId = 1, ApplicationUserId = user.Id });
            await context.SaveChangesAsync();
            
            var controller = new ContractRatesController(context);

            // Act
            var result = await controller.GetContractRate(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ContractRate>>(result);
            var returnValue = Assert.IsType<ContractRate>(actionResult.Value);
            Assert.Equal(1, returnValue.Id);
            Assert.Equal("Rate 1", returnValue.Description);
            Assert.Equal(100m, returnValue.Rate);
            Assert.Equal(user.Id, returnValue.ApplicationUserId);
            Assert.NotNull(returnValue.ApplicationUser);
            Assert.Equal("testuser2", returnValue.ApplicationUser.UserName);
        }
    }
}
