using Xunit;
using comaiz.Extensions;

namespace comaiz.Tests
{
    public class StringExtensionsTests
    {
        [Fact]
        public void GetNpgsqlConnectionString_UriFormat_ReturnsExpectedConnectionString()
        {
            // Arrange
            string configuredConnectionString = "postgres://username:password@localhost:5432/my_db";

            // Act
            string result = configuredConnectionString.GetNpgsqlConnectionString();

            // Assert
            Assert.Equal("SSL Mode=VerifyFull;Host=localhost;Port=5432;Database=my_db;Username=username;Password=password", result);
        }

        [Fact]
        public void GetNpgsqlConnectionString_ConnectionStringFormat_ReturnsExpectedConnectionString()
        {
            // Arrange
            string configuredConnectionString = "Host=localhost;Port=5432;Username=username;Password=password;Database=my_db";

            // Act
            string result = configuredConnectionString.GetNpgsqlConnectionString();

            // Assert
            Assert.Equal("Host=localhost;Port=5432;Username=username;Password=password;Database=my_db", result);
        }
    }
}
