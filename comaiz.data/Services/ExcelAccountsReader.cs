using System.Data;
using comaiz.data.Models;
using ExcelDataReader;

namespace comaiz.data.Services
{
    public class ExcelAccountsReader
    {
        private DataSet? _accountsData;

        public IEnumerable<Contract> GetContracts()
        {
            return (_accountsData?.Tables["Contracts"] ?? throw new InvalidOperationException())
                .AsEnumerable().Select(row =>
                    new Contract
                    {
                        //ChargeType = ChargeType.TimeAndMaterials,
                        Description = row["Name"] as string,
                        // Add a contract rate for the hourly rate
                        ContractRates = new[]
                        {
                            new ContractRate
                            {
                                Description = "Hourly rate",
                                Rate = Convert.ToDecimal(row["HourlyRate"])
                            }
                        },
                        Price = Convert.ToDecimal(row["Contract Value"]),
                        Schedule = row["Assignment"] as string,
                    });
        }

        public void Load(string excelFile)
        {
            // This required for the ExcelDataReader to work in .net core
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using var stream = File.Open(@"C:\Users\danbo\Google Drive\Misc\Comaiz\Accounts.xlsx", FileMode.Open,
                FileAccess.Read);
            using var reader = ExcelReaderFactory.CreateReader(stream);

            _accountsData = reader.AsDataSet(new ExcelDataSetConfiguration()
            {
                ConfigureDataTable = _ =>
                    new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = true
                    }
            });
        }

        public IEnumerable<Client> GetClients()
        {
            return new[]
            {
                new Client
                {
                    Name = "Some client",
                    ShortName = "Client"
                }
            };
        }
    }
}
