using System.Data;
using System.Security.Cryptography.X509Certificates;
using comaiz.Models;
using comaiz.Services;
using ExcelDataReader;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace comaiz.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ComaizContext context)
        {
            if (context.Clients.Any())
                return;

            var hta = new Client { Name = "HORIBA Test Automation", ShortName = "HTA" };
            var ifis = new Client { Name = "Institute of food science", ShortName = "IFIS" };
            context.Clients.Add(hta);
            context.Clients.Add(ifis);
            context.SaveChanges();

            if (context.Contracts != null)
                context.Contracts.Add(new Contract { Client = hta });
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
            var hta = context.Clients.FirstOrDefault(c => c.ShortName == "HTA");

            foreach (var contract in excelReader.GetContracts())
            {
                contract.Client = hta;
                context.Contracts?.Add(contract);
            }

            context.SaveChanges();
        }

    }
}
