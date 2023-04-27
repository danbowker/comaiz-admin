using System.Data;
using comaiz.Models;
using ExcelDataReader;

namespace comaiz.Services
{
    public class ExcelAccountsReader
    {
        public IEnumerable<Contract> GetContracts()
        {
            using var stream = File.Open(@"C:\Users\danbo\Google Drive\Misc\Comaiz\Accounts.xlsx", FileMode.Open,
                FileAccess.Read);
            using var reader = ExcelReaderFactory.CreateReader(stream);

            var result = reader.AsDataSet(new ExcelDataSetConfiguration()
            {
                ConfigureDataTable = _ =>
                    new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = true
                    }
            });
            return (result.Tables["Contracts"] ?? throw new InvalidOperationException())
                .AsEnumerable().Select(row =>
                    new Contract
                    {
                        ChargeType = ChargeType.TimeAndMaterials, 
                        Description = row["Name"] as string,
                    });
        }
    }
}
