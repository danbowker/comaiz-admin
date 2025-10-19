using Xunit;
using Microsoft.EntityFrameworkCore;
using comaiz.api.Controllers;
using comaiz.data;
using comaiz.data.Models;
using Microsoft.AspNetCore.Mvc;

namespace comaiz.tests.Controllers
{
    /// <summary>
    /// Unit tests for WorkersController using In-Memory Database.
    /// 
    /// Testing Approach:
    /// - Uses Entity Framework Core In-Memory Database for testing
    /// - Each test gets a unique database instance to ensure isolation
    /// - Tests do not interact with a real database (PostgreSQL)
    /// - Covers all CRUD operations (Create, Read, Update, Delete)
    /// - Tests both success and failure scenarios
    /// - Verifies proper HTTP status codes and responses
    /// 
    /// How to Run:
    /// - Run all tests: dotnet test
    /// - Run this specific test file: dotnet test --filter WorkersControllerTests
    /// - Run in CI pipeline: Tests automatically run with 'dotnet test' command
    /// </summary>
    public class WorkersControllerTests
    {
        private ComaizContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ComaizContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            return new ComaizContext(options);
        }

        [Fact]
        public async Task GetWorkers_ReturnsAllWorkers()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            context.Workers!.Add(new Worker { Id = 1, Name = "John Doe" });
            context.Workers.Add(new Worker { Id = 2, Name = "Jane Smith" });
            await context.SaveChangesAsync();
            
            var controller = new WorkersController(context);

            // Act
            var result = await controller.GetWorkers();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Worker>>>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<Worker>>(actionResult.Value);
            Assert.Equal(2, returnValue.Count());
        }

        [Fact]
        public async Task GetWorker_WithValidId_ReturnsWorker()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            context.Workers!.Add(new Worker { Id = 1, Name = "John Doe" });
            await context.SaveChangesAsync();
            
            var controller = new WorkersController(context);

            // Act
            var result = await controller.GetWorker(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Worker>>(result);
            var returnValue = Assert.IsType<Worker>(actionResult.Value);
            Assert.Equal(1, returnValue.Id);
            Assert.Equal("John Doe", returnValue.Name);
        }

        [Fact]
        public async Task GetWorker_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var controller = new WorkersController(context);

            // Act
            var result = await controller.GetWorker(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task PostWorker_WithValidWorker_ReturnsCreatedAtAction()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var worker = new Worker { Name = "John Doe" };
            var controller = new WorkersController(context);

            // Act
            var result = await controller.PostWorker(worker);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Worker>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            Assert.Equal("GetWorker", createdAtActionResult.ActionName);
            Assert.Equal(worker.Name, ((Worker)createdAtActionResult.Value!).Name);
            
            // Verify it was actually saved
            var savedWorker = await context.Workers!.FindAsync(worker.Id);
            Assert.NotNull(savedWorker);
            Assert.Equal("John Doe", savedWorker.Name);
        }

        [Fact]
        public async Task PutWorker_WithValidWorker_ReturnsNoContent()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var worker = new Worker { Id = 1, Name = "John Doe" };
            context.Workers!.Add(worker);
            await context.SaveChangesAsync();
            
            // Detach to simulate a new request
            context.Entry(worker).State = EntityState.Detached;
            
            var updatedWorker = new Worker { Id = 1, Name = "John Doe Updated" };
            var controller = new WorkersController(context);

            // Act
            var result = await controller.PutWorker(updatedWorker);

            // Assert
            Assert.IsType<NoContentResult>(result);
            
            // Verify the update
            var savedWorker = await context.Workers.FindAsync(1);
            Assert.Equal("John Doe Updated", savedWorker!.Name);
        }

        [Fact]
        public async Task PutWorker_WhenWorkerNotExists_ReturnsNotFound()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var worker = new Worker { Id = 999, Name = "John Doe" };
            var controller = new WorkersController(context);

            // Act
            var result = await controller.PutWorker(worker);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteWorker_WithValidId_ReturnsNoContent()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var worker = new Worker { Id = 1, Name = "John Doe" };
            context.Workers!.Add(worker);
            await context.SaveChangesAsync();
            
            var controller = new WorkersController(context);

            // Act
            var result = await controller.DeleteWorker(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            
            // Verify deletion
            var deletedWorker = await context.Workers.FindAsync(1);
            Assert.Null(deletedWorker);
        }

        [Fact]
        public async Task DeleteWorker_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var controller = new WorkersController(context);

            // Act
            var result = await controller.DeleteWorker(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
