using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using comaiz.data;
using DotNet.Testcontainers.Builders;
using Testcontainers.PostgreSql;
using comaiz.data.Models;
using Microsoft.AspNetCore.Identity;

namespace comaiz.tests.IntegrationTests;

public class ComaizApiWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private PostgreSqlContainer? _postgresContainer;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContext configuration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ComaizContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add DbContext with the test database connection string
            services.AddDbContext<ComaizContext>(options =>
            {
                var connectionString = _postgresContainer?.GetConnectionString();
                options.UseNpgsql(connectionString);
            });

            // Build the service provider
            var serviceProvider = services.BuildServiceProvider();

            // Create a scope to obtain a reference to the database context
            using var scope = serviceProvider.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<ComaizContext>();

            // Ensure the database is created and apply migrations
            db.Database.EnsureCreated();
            
            // Seed test users and roles using TestDataSeeder
            TestDataSeeder.CreateTestUserAndGetToken(scopedServices).GetAwaiter().GetResult();
        });
    }

    public async System.Threading.Tasks.Task InitializeAsync()
    {
        _postgresContainer = new PostgreSqlBuilder()
            .WithImage("postgres:16")
            .WithDatabase("comaiz_test")
            .WithUsername("test_user")
            .WithPassword("test_password")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5432))
            .Build();

        await _postgresContainer.StartAsync();
    }

    public new async System.Threading.Tasks.Task DisposeAsync()
    {
        if (_postgresContainer != null)
        {
            await _postgresContainer.DisposeAsync();
        }
        await base.DisposeAsync();
    }
}
