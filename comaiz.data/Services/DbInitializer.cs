using comaiz.data.Models;

namespace comaiz.data.Services
{
    public static class DbInitializer
    {
        public static void Initialize(ComaizContext context)
        {
            // Look for any clients.
            if (context.Clients?.Any() ?? false)
            {
                return;   // DB has been seeded
            }
            var client1 = new Client { Name = "Some client Ltd.", ShortName = "Client1" };
            var client2 = new Client { Name = "Another client Ltd.", ShortName = "Client2" };
            context.Clients?.Add(client1);
            context.Clients?.Add(client2);
            context.SaveChanges();
            context.Contracts?.Add(new Contract { Client = client1 });
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
            var client1 = context.Clients?.FirstOrDefault(c => c.ShortName == "Client1");

            foreach (var contract in excelReader.GetContracts())
            {
                contract.Client = client1;
                context?.Contracts?.Add(contract);
            }

            context?.SaveChanges();
        }

    }
}
