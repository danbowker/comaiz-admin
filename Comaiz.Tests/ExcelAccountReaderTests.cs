using comaiz.Services;

namespace Comaiz.Tests
{
    public class ExcelAccountReaderTests
    {
        [Fact]
        public void Test1()
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            var reader = new ExcelAccountsReader();
            Assert.True(reader.GetContracts().Count() > 0);
            
        }
    }
}