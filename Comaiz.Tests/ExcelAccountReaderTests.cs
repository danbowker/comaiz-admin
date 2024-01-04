using comaiz.data.Services;

namespace Comaiz.Tests
{
    public class ExcelAccountReaderTests
    {
        [Fact]
        public void GetContracts_ReturnsSomeContracts()
        {
            var excelReader = new ExcelAccountsReader();
            excelReader.Load(@"C:\Users\danbo\Google Drive\Misc\Comaiz\Accounts.xlsx");
            Assert.True(excelReader.GetContracts().Count() > 0);
        }

        [Fact]
        public void GetContracts_AllContractsHaveADescription()
        {
            var excelReader = new ExcelAccountsReader();
            excelReader.Load(@"C:\Users\danbo\Google Drive\Misc\Comaiz\Accounts.xlsx");
            Assert.True(excelReader.GetContracts().All(c => !string.IsNullOrEmpty(c.Description)));
        }

        // Test that GetContracts returns all contracts with a valid rate
        [Fact]
        public void GetContracts_AllContractsHaveAValidRate()
        {
            var excelReader = new ExcelAccountsReader();
            excelReader.Load(@"C:\Users\danbo\Google Drive\Misc\Comaiz\Accounts.xlsx");
            Assert.True(excelReader.GetContracts().All(c => c.Rate > 0));
        }
    }
}