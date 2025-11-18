using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using comaiz.api.Controllers;
using comaiz.data;
using comaiz.data.Models;
using Microsoft.AspNetCore.Mvc;

namespace comaiz.tests.Controllers
{
    /// <summary>
    /// Unit tests for ClientsController using In-Memory Database.
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
    /// - Run this specific test file: dotnet test --filter ClientsControllerTests
    /// - Run in CI pipeline: Tests automatically run with 'dotnet test' command
    /// </summary>
    public class ClientsControllerTests
    {
        private ComaizContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ComaizContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            return new ComaizContext(options);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetClientsAsync_ReturnsAllClients()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            context.Clients!.Add(new Client { Id = 1, ShortName = "ABC", Name = "ABC Company" });
            context.Clients.Add(new Client { Id = 2, ShortName = "XYZ", Name = "XYZ Corporation" });
            await context.SaveChangesAsync();
            
            var controller = new ClientsController(context);

            // Act
            var result = await controller.GetClientsAsync();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Client>>>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<Client>>(actionResult.Value);
            Assert.Equal(2, returnValue.Count());
        }

        [Fact]
        public async System.Threading.Tasks.Task GetClientAsync_WithValidId_ReturnsClient()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            context.Clients!.Add(new Client { Id = 1, ShortName = "ABC", Name = "ABC Company" });
            await context.SaveChangesAsync();
            
            var controller = new ClientsController(context);

            // Act
            var result = await controller.GetClientAsync(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Client>>(result);
            var returnValue = Assert.IsType<Client>(actionResult.Value);
            Assert.Equal(1, returnValue.Id);
            Assert.Equal("ABC", returnValue.ShortName);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetClientAsync_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var controller = new ClientsController(context);

            // Act
            var result = await controller.GetClientAsync(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async System.Threading.Tasks.Task PostClient_WithValidClient_ReturnsCreatedAtAction()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var client = new Client { ShortName = "ABC", Name = "ABC Company" };
            var controller = new ClientsController(context);

            // Act
            var result = await controller.PostClient(client);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Client>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            Assert.Equal("GetClient", createdAtActionResult.ActionName);
            Assert.Equal(client.ShortName, ((Client)createdAtActionResult.Value!).ShortName);
            
            // Verify it was actually saved
            var savedClient = await context.Clients!.FindAsync(client.Id);
            Assert.NotNull(savedClient);
            Assert.Equal("ABC", savedClient.ShortName);
        }

        [Fact]
        public async System.Threading.Tasks.Task PutClient_WithValidClient_ReturnsNoContent()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var client = new Client { Id = 1, ShortName = "ABC", Name = "ABC Company" };
            context.Clients!.Add(client);
            await context.SaveChangesAsync();
            
            // Detach to simulate a new request
            context.Entry(client).State = EntityState.Detached;
            
            var updatedClient = new Client { Id = 1, ShortName = "ABC", Name = "ABC Company Updated" };
            var controller = new ClientsController(context);

            // Act
            var result = await controller.PutClient(updatedClient);

            // Assert
            Assert.IsType<NoContentResult>(result);
            
            // Verify the update
            var savedClient = await context.Clients.FindAsync(1);
            Assert.Equal("ABC Company Updated", savedClient!.Name);
        }

        [Fact]
        public async System.Threading.Tasks.Task PutClient_WhenClientNotExists_ReturnsNotFound()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var client = new Client { Id = 999, ShortName = "ABC", Name = "ABC Company" };
            var controller = new ClientsController(context);

            // Act
            var result = await controller.PutClient(client);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async System.Threading.Tasks.Task DeleteClient_WithValidId_ReturnsNoContent()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var client = new Client { Id = 1, ShortName = "ABC", Name = "ABC Company" };
            context.Clients!.Add(client);
            await context.SaveChangesAsync();
            
            var controller = new ClientsController(context);

            // Act
            var result = await controller.DeleteClient(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            
            // Verify deletion
            var deletedClient = await context.Clients.FindAsync(1);
            Assert.Null(deletedClient);
        }

        [Fact]
        public async System.Threading.Tasks.Task DeleteClient_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var controller = new ClientsController(context);

            // Act
            var result = await controller.DeleteClient(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async System.Threading.Tasks.Task DuplicateClient_WithValidId_ReturnsCreatedAtAction()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var client = new Client { Id = 1, ShortName = "ABC", Name = "ABC Company" };
            context.Clients!.Add(client);
            await context.SaveChangesAsync();
            
            var controller = new ClientsController(context);

            // Act
            var result = await controller.DuplicateClient(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Client>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            Assert.Equal("GetClient", createdAtActionResult.ActionName);
            
            var duplicatedClient = Assert.IsType<Client>(createdAtActionResult.Value);
            Assert.NotEqual(1, duplicatedClient.Id);
            Assert.Equal("ABC", duplicatedClient.ShortName);
            Assert.Equal("ABC Company (Copy)", duplicatedClient.Name);
            
            // Verify both clients exist in the database
            var allClients = await context.Clients.ToListAsync();
            Assert.Equal(2, allClients.Count);
        }

        [Fact]
        public async System.Threading.Tasks.Task DuplicateClient_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var controller = new ClientsController(context);

            // Act
            var result = await controller.DuplicateClient(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }
    }
}
