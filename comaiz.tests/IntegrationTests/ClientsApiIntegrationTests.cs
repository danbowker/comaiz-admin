using System.Net;
using System.Net.Http.Json;
using comaiz.data;
using comaiz.data.Models;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace comaiz.tests.IntegrationTests;

public class ClientsApiIntegrationTests : IClassFixture<ComaizApiWebApplicationFactory>, IAsyncLifetime
{
    private readonly ComaizApiWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public ClientsApiIntegrationTests(ComaizApiWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        // Seed test data before each test
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ComaizContext>();
        TestDataSeeder.SeedTestData(context);
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        // Clean up after each test
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ComaizContext>();
        TestDataSeeder.ClearTestData(context);
        await Task.CompletedTask;
    }

    [Fact]
    public async Task GetClients_ReturnsSuccessAndAllClients()
    {
        // Act
        var response = await _client.GetAsync("/api/clients");

        // Assert
        response.EnsureSuccessStatusCode();
        var clients = await response.Content.ReadFromJsonAsync<List<Client>>();
        
        Assert.NotNull(clients);
        Assert.Equal(3, clients.Count);
        Assert.Contains(clients, c => c.ShortName == "ACME");
        Assert.Contains(clients, c => c.ShortName == "TECH");
        Assert.Contains(clients, c => c.ShortName == "INNO");
    }

    [Fact]
    public async Task GetClient_WithValidId_ReturnsClient()
    {
        // Arrange - Get an existing client ID
        var allClients = await _client.GetFromJsonAsync<List<Client>>("/api/clients");
        var existingClientId = allClients!.First().Id;

        // Act
        var response = await _client.GetAsync($"/api/clients/{existingClientId}");

        // Assert
        response.EnsureSuccessStatusCode();
        var client = await response.Content.ReadFromJsonAsync<Client>();
        
        Assert.NotNull(client);
        Assert.Equal(existingClientId, client.Id);
    }

    [Fact]
    public async Task GetClient_WithInvalidId_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/clients/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PostClient_WithValidData_CreatesNewClient()
    {
        // Arrange
        var newClient = new Client
        {
            ShortName = "NEW",
            Name = "New Client Corp"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/clients", newClient);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        var createdClient = await response.Content.ReadFromJsonAsync<Client>();
        Assert.NotNull(createdClient);
        Assert.Equal("NEW", createdClient.ShortName);
        Assert.Equal("New Client Corp", createdClient.Name);
        Assert.True(createdClient.Id > 0);

        // Verify it was actually created
        var getResponse = await _client.GetAsync($"/api/clients/{createdClient.Id}");
        getResponse.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task PutClient_WithValidData_UpdatesClient()
    {
        // Arrange - Get an existing client
        var allClients = await _client.GetFromJsonAsync<List<Client>>("/api/clients");
        var clientToUpdate = allClients!.First();
        
        clientToUpdate.ShortName = "UPDATED";
        clientToUpdate.Name = "Updated Client Name";

        // Act
        var response = await _client.PutAsJsonAsync("/api/clients", clientToUpdate);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify the update
        var updatedClient = await _client.GetFromJsonAsync<Client>($"/api/clients/{clientToUpdate.Id}");
        Assert.NotNull(updatedClient);
        Assert.Equal("UPDATED", updatedClient.ShortName);
        Assert.Equal("Updated Client Name", updatedClient.Name);
    }

    [Fact]
    public async Task DeleteClient_WithValidId_DeletesClient()
    {
        // Arrange - Get an existing client
        var allClients = await _client.GetFromJsonAsync<List<Client>>("/api/clients");
        var clientToDelete = allClients!.First();

        // Act
        var response = await _client.DeleteAsync($"/api/clients/{clientToDelete.Id}");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify it was deleted
        var getResponse = await _client.GetAsync($"/api/clients/{clientToDelete.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteClient_WithInvalidId_ReturnsNotFound()
    {
        // Act
        var response = await _client.DeleteAsync("/api/clients/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
