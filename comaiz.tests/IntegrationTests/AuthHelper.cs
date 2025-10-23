using System.Net.Http.Headers;
using System.Net.Http.Json;
using comaiz.api.Models;

namespace comaiz.tests.IntegrationTests;

public static class AuthHelper
{
    public static async Task<string> GetAuthTokenAsync(HttpClient client)
    {
        var loginRequest = new LoginRequest
        {
            Username = "testuser",
            Password = "Test@123"
        };

        var response = await client.PostAsJsonAsync("/api/auth/login", loginRequest);
        response.EnsureSuccessStatusCode();

        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
        return authResponse?.Token ?? throw new InvalidOperationException("Failed to get auth token");
    }

    public static void SetAuthToken(HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}
