using comaiz.data;
using comaiz.data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace comaiz.tests.IntegrationTests;

public static class TestDataSeeder
{
    public static async Task<string> CreateTestUserAndGetToken(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        
        // Create Admin role if it doesn't exist
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new ApplicationRole { Name = "Admin" });
        }
        
        // Create User role if it doesn't exist
        if (!await roleManager.RoleExistsAsync("User"))
        {
            await roleManager.CreateAsync(new ApplicationRole { Name = "User" });
        }

        // Create test user
        var testUsername = "testuser";
        var testUser = await userManager.FindByNameAsync(testUsername);
        
        if (testUser == null)
        {
            testUser = new ApplicationUser
            {
                UserName = testUsername,
                Email = "testuser@test.com",
                EmailConfirmed = true
            };
            
            await userManager.CreateAsync(testUser, "Test@123");
            await userManager.AddToRoleAsync(testUser, "User");
        }

        // Generate token using a login request
        // We'll return the username/password so tests can call /api/auth/login
        return "testuser|Test@123";
    }

    public static void SeedTestData(ComaizContext context)
    {
        // Clear any existing data
        context.Clients?.RemoveRange(context.Clients);
        context.Workers?.RemoveRange(context.Workers);
        context.SaveChanges();

        // Seed Clients
        var clients = new List<Client>
        {
            new Client { ShortName = "ACME", Name = "Acme Corporation" },
            new Client { ShortName = "TECH", Name = "TechVision Inc" },
            new Client { ShortName = "INNO", Name = "Innovation Labs" }
        };
        context.Clients?.AddRange(clients);

        // Seed Workers
        var workers = new List<Worker>
        {
            new Worker { Name = "John Doe" },
            new Worker { Name = "Jane Smith" },
            new Worker { Name = "Bob Johnson" }
        };
        context.Workers?.AddRange(workers);

        context.SaveChanges();
    }

    public static void ClearTestData(ComaizContext context)
    {
        // Remove all test data
        if (context.InvoiceItems != null)
            context.InvoiceItems.RemoveRange(context.InvoiceItems);
        if (context.Invoices != null)
            context.Invoices.RemoveRange(context.Invoices);
        if (context.WorkRecords != null)
            context.WorkRecords.RemoveRange(context.WorkRecords);
        if (context.ContractRates != null)
            context.ContractRates.RemoveRange(context.ContractRates);
        if (context.Contracts != null)
            context.Contracts.RemoveRange(context.Contracts);
        if (context.FixedCosts != null)
            context.FixedCosts.RemoveRange(context.FixedCosts);
        if (context.CarJourneys != null)
            context.CarJourneys.RemoveRange(context.CarJourneys);
        if (context.Clients != null)
            context.Clients.RemoveRange(context.Clients);
        if (context.Workers != null)
            context.Workers.RemoveRange(context.Workers);

        context.SaveChanges();
    }
}
