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
        
        // Ensure default roles exist using the centralized DatabaseSeeder
        await comaiz.api.Services.DatabaseSeeder.SeedDefaultRoles(serviceProvider);

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
        context.Contracts?.RemoveRange(context.Contracts);
        context.ContractRates?.RemoveRange(context.ContractRates);
        context.Tasks?.RemoveRange(context.Tasks);
        context.TaskContractRates?.RemoveRange(context.TaskContractRates);
        context.SaveChanges();

        // Seed Clients
        var clients = new List<Client>
        {
            new Client { ShortName = "ACME", Name = "Acme Corporation" },
            new Client { ShortName = "TECH", Name = "TechVision Inc" },
            new Client { ShortName = "INNO", Name = "Innovation Labs" }
        };
        context.Clients?.AddRange(clients);
        context.SaveChanges();

        // Seed Contracts
        var contracts = new List<Contract>
        {
            new Contract { ClientId = clients[0].Id, Description = "ACME Contract", ChargeType = ChargeType.TimeAndMaterials },
            new Contract { ClientId = clients[1].Id, Description = "TECH Contract", ChargeType = ChargeType.TimeAndMaterials },
        };
        context.Contracts?.AddRange(contracts);
        context.SaveChanges();

        // Seed Contract Rates
        var contractRates = new List<ContractRate>
        {
            new ContractRate { ContractId = contracts[0].Id, Description = "Developer Rate", Rate = 100 },
            new ContractRate { ContractId = contracts[0].Id, Description = "Tester Rate", Rate = 80 },
            new ContractRate { ContractId = contracts[0].Id, Description = "Designer Rate", Rate = 90 },
            new ContractRate { ContractId = contracts[1].Id, Description = "Senior Dev Rate", Rate = 120 },
        };
        context.ContractRates?.AddRange(contractRates);

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
        if (context.TaskContractRates != null)
            context.TaskContractRates.RemoveRange(context.TaskContractRates);
        if (context.Tasks != null)
            context.Tasks.RemoveRange(context.Tasks);
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
