using comaiz.data;
using comaiz.data.Models;

namespace comaiz.tests.IntegrationTests;

public static class TestDataSeeder
{
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
