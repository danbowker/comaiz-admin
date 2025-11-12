using System.Net;
using System.Net.Http.Json;
using comaiz.data;
using comaiz.data.Models;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace comaiz.tests.IntegrationTests;

public class TasksApiIntegrationTests : IClassFixture<ComaizApiWebApplicationFactory>, IAsyncLifetime
{
    private readonly ComaizApiWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private string _authToken = string.Empty;

    public TasksApiIntegrationTests(ComaizApiWebApplicationFactory factory)
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
    public async System.Threading.Tasks.Task PostTask_WithTaskContractRates_CreatesTaskWithMultipleRates()
    {
        // Arrange - Get test data
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ComaizContext>();
        
        var contract = context.Contracts!.First();
        var contractRates = context.ContractRates!.Where(cr => cr.ContractId == contract.Id).Take(2).ToList();

        var newTask = new comaiz.data.Models.Task
        {
            Name = "Integration Test Task",
            ContractId = contract.Id,
            TaskContractRates = contractRates.Select(cr => new TaskContractRate
            {
                ContractRateId = cr.Id
            }).ToList()
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/tasks", newTask);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        var createdTask = await response.Content.ReadFromJsonAsync<comaiz.data.Models.Task>();
        Assert.NotNull(createdTask);
        Assert.Equal("Integration Test Task", createdTask.Name);
        Assert.True(createdTask.Id > 0);

        // Verify it was created with the correct contract rates
        var getResponse = await _client.GetAsync($"/api/tasks/{createdTask.Id}");
        getResponse.EnsureSuccessStatusCode();
        var retrievedTask = await getResponse.Content.ReadFromJsonAsync<comaiz.data.Models.Task>();
        Assert.NotNull(retrievedTask);
        Assert.NotNull(retrievedTask.TaskContractRates);
        Assert.Equal(2, retrievedTask.TaskContractRates.Count);
    }

    [Fact]
    public async System.Threading.Tasks.Task PutTask_UpdatesTaskContractRates()
    {
        // Arrange - Create a task first
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ComaizContext>();
        
        var contract = context.Contracts!.First();
        var allContractRates = context.ContractRates!.Where(cr => cr.ContractId == contract.Id).ToList();
        var initialRate = allContractRates.First();

        var task = new comaiz.data.Models.Task
        {
            Name = "Task to Update",
            ContractId = contract.Id,
            TaskContractRates = new List<TaskContractRate>
            {
                new TaskContractRate { ContractRateId = initialRate.Id }
            }
        };

        var createResponse = await _client.PostAsJsonAsync("/api/tasks", task);
        createResponse.EnsureSuccessStatusCode();
        var createdTask = await createResponse.Content.ReadFromJsonAsync<comaiz.data.Models.Task>();

        // Update with different contract rates
        createdTask!.Name = "Updated Task Name";
        createdTask.TaskContractRates = allContractRates.Skip(1).Take(2).Select(cr => new TaskContractRate
        {
            TaskId = createdTask.Id,
            ContractRateId = cr.Id
        }).ToList();

        // Act
        var updateResponse = await _client.PutAsJsonAsync("/api/tasks", createdTask);

        // Assert
        updateResponse.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

        // Verify the update
        var getResponse = await _client.GetAsync($"/api/tasks/{createdTask.Id}");
        var updatedTask = await getResponse.Content.ReadFromJsonAsync<comaiz.data.Models.Task>();
        Assert.NotNull(updatedTask);
        Assert.Equal("Updated Task Name", updatedTask.Name);
        Assert.NotNull(updatedTask.TaskContractRates);
        Assert.Equal(2, updatedTask.TaskContractRates.Count);
        Assert.DoesNotContain(updatedTask.TaskContractRates, tcr => tcr.ContractRateId == initialRate.Id);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetTasks_ReturnsTasksWithContractRates()
    {
        // Arrange - Create a task with contract rates
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ComaizContext>();
        
        var contract = context.Contracts!.First();
        var contractRate = context.ContractRates!.First(cr => cr.ContractId == contract.Id);

        var task = new comaiz.data.Models.Task
        {
            Name = "Test Task",
            ContractId = contract.Id,
            TaskContractRates = new List<TaskContractRate>
            {
                new TaskContractRate { ContractRateId = contractRate.Id }
            }
        };

        await _client.PostAsJsonAsync("/api/tasks", task);

        // Act
        var response = await _client.GetAsync("/api/tasks");

        // Assert
        response.EnsureSuccessStatusCode();
        var tasks = await response.Content.ReadFromJsonAsync<List<comaiz.data.Models.Task>>();
        
        Assert.NotNull(tasks);
        Assert.Contains(tasks, t => t.Name == "Test Task");
        var createdTask = tasks.First(t => t.Name == "Test Task");
        Assert.NotNull(createdTask.TaskContractRates);
        Assert.Single(createdTask.TaskContractRates);
    }

    [Fact]
    public async System.Threading.Tasks.Task DeleteTask_RemovesTaskAndContractRates()
    {
        // Arrange - Create a task
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ComaizContext>();
        
        var contract = context.Contracts!.First();
        var contractRate = context.ContractRates!.First(cr => cr.ContractId == contract.Id);

        var task = new comaiz.data.Models.Task
        {
            Name = "Task to Delete",
            ContractId = contract.Id,
            TaskContractRates = new List<TaskContractRate>
            {
                new TaskContractRate { ContractRateId = contractRate.Id }
            }
        };

        var createResponse = await _client.PostAsJsonAsync("/api/tasks", task);
        var createdTask = await createResponse.Content.ReadFromJsonAsync<comaiz.data.Models.Task>();

        // Act
        var deleteResponse = await _client.DeleteAsync($"/api/tasks/{createdTask!.Id}");

        // Assert
        deleteResponse.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        // Verify it was deleted
        var getResponse = await _client.GetAsync($"/api/tasks/{createdTask.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);

        // Verify TaskContractRates were also deleted (cascade)
        using var verifyScope = _factory.Services.CreateScope();
        var verifyContext = verifyScope.ServiceProvider.GetRequiredService<ComaizContext>();
        var remainingTaskContractRates = verifyContext.TaskContractRates!.Count(tcr => tcr.TaskId == createdTask.Id);
        Assert.Equal(0, remainingTaskContractRates);
    }
}
