using System.Net;
using System.Net.Http.Json;
using comaiz.data;
using comaiz.data.Models;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace comaiz.tests.IntegrationTests;

public class WorkersApiIntegrationTests : IClassFixture<ComaizApiWebApplicationFactory>, IAsyncLifetime
{
    private readonly ComaizApiWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private string _authToken = string.Empty;

    public WorkersApiIntegrationTests(ComaizApiWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public async System.Threading.Tasks.Task InitializeAsync()
    {
        // Get auth token first
        _authToken = await AuthHelper.GetAuthTokenAsync(_client);
        AuthHelper.SetAuthToken(_client, _authToken);

        // Seed test data before each test
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ComaizContext>();
        TestDataSeeder.SeedTestData(context);
        await System.Threading.Tasks.Task.CompletedTask;
    }

    public async System.Threading.Tasks.Task DisposeAsync()
    {
        // Clean up after each test
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ComaizContext>();
        TestDataSeeder.ClearTestData(context);
        await System.Threading.Tasks.Task.CompletedTask;
    }

    [Fact]
    public async System.Threading.Tasks.Task GetWorkers_ReturnsSuccessAndAllWorkers()
    {
        // Act
        var response = await _client.GetAsync("/api/workers");

        // Assert
        response.EnsureSuccessStatusCode();
        var workers = await response.Content.ReadFromJsonAsync<List<Worker>>();
        
        Assert.NotNull(workers);
        Assert.Equal(3, workers.Count);
        Assert.Contains(workers, w => w.Name == "John Doe");
        Assert.Contains(workers, w => w.Name == "Jane Smith");
        Assert.Contains(workers, w => w.Name == "Bob Johnson");
    }

    [Fact]
    public async System.Threading.Tasks.Task GetWorker_WithValidId_ReturnsWorker()
    {
        // Arrange - Get an existing worker ID
        var allWorkers = await _client.GetFromJsonAsync<List<Worker>>("/api/workers");
        var existingWorkerId = allWorkers!.First().Id;

        // Act
        var response = await _client.GetAsync($"/api/workers/{existingWorkerId}");

        // Assert
        response.EnsureSuccessStatusCode();
        var worker = await response.Content.ReadFromJsonAsync<Worker>();
        
        Assert.NotNull(worker);
        Assert.Equal(existingWorkerId, worker.Id);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetWorker_WithInvalidId_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/workers/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async System.Threading.Tasks.Task PostWorker_WithValidData_CreatesNewWorker()
    {
        // Arrange
        var newWorker = new Worker
        {
            Name = "Alice Williams"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/workers", newWorker);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        var createdWorker = await response.Content.ReadFromJsonAsync<Worker>();
        Assert.NotNull(createdWorker);
        Assert.Equal("Alice Williams", createdWorker.Name);
        Assert.True(createdWorker.Id > 0);

        // Verify it was actually created
        var getResponse = await _client.GetAsync($"/api/workers/{createdWorker.Id}");
        getResponse.EnsureSuccessStatusCode();
    }

    [Fact]
    public async System.Threading.Tasks.Task PutWorker_WithValidData_UpdatesWorker()
    {
        // Arrange - Get an existing worker
        var allWorkers = await _client.GetFromJsonAsync<List<Worker>>("/api/workers");
        var workerToUpdate = allWorkers!.First();
        
        workerToUpdate.Name = "Updated Worker Name";

        // Act
        var response = await _client.PutAsJsonAsync("/api/workers", workerToUpdate);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify the update
        var updatedWorker = await _client.GetFromJsonAsync<Worker>($"/api/workers/{workerToUpdate.Id}");
        Assert.NotNull(updatedWorker);
        Assert.Equal("Updated Worker Name", updatedWorker.Name);
    }

    [Fact]
    public async System.Threading.Tasks.Task DeleteWorker_WithValidId_DeletesWorker()
    {
        // Arrange - Get an existing worker
        var allWorkers = await _client.GetFromJsonAsync<List<Worker>>("/api/workers");
        var workerToDelete = allWorkers!.First();

        // Act
        var response = await _client.DeleteAsync($"/api/workers/{workerToDelete.Id}");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify it was deleted
        var getResponse = await _client.GetAsync($"/api/workers/{workerToDelete.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async System.Threading.Tasks.Task DeleteWorker_WithInvalidId_ReturnsNotFound()
    {
        // Act
        var response = await _client.DeleteAsync("/api/workers/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetWorkers_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange - Create a new client without auth token
        var unauthenticatedClient = _factory.CreateClient();

        // Act
        var response = await unauthenticatedClient.GetAsync("/api/workers");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
