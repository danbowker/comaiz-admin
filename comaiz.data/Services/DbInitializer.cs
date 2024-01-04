using comaiz.data.Models;

namespace comaiz.data.Services
{
    public static class DbInitializer
    {
        public static void Initialize(ComaizContext context)
        {
            context.Database.EnsureCreated();
            // Look for any clients.
            if (context.Clients?.Any() ?? false)
            {
                return;   // DB has been seeded
            }
            var hta = new Client { Name = "HORIBA Test Automation", ShortName = "HTA" };
            var ifis = new Client { Name = "Institute of food science", ShortName = "IFIS" };
            context.Clients?.Add(hta);
            context.Clients?.Add(ifis);
            context.SaveChanges();
            context.Contracts?.Add(new Contract { Client = hta });
            context.SaveChanges();
        }

        public static void ImportFromExcel(ComaizContext context)
        {
            var excelReader = new ExcelAccountsReader();
            excelReader.Load(@"C:\Users\danbo\Google Drive\Misc\Comaiz\Accounts.xlsx");

            foreach (var client in excelReader.GetClients())
            {
                context.Clients?.Add(client);
            }
            var hta = context.Clients?.FirstOrDefault(c => c.ShortName == "HTA");

            foreach (var contract in excelReader.GetContracts())
            {
                contract.Client = hta;
                context?.Contracts?.Add(contract);
            }

            context?.SaveChanges();
        }

    }
}
