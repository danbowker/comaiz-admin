using Npgsql;

namespace comaiz.Extensions
{
    public static class StringExtensions
    {
        public static string GetNpgsqlConnectionString(this string? configuredConnectionString)
        {
            NpgsqlConnectionStringBuilder connStringBuilder;

            // If the connection string is a URI, parse it and use the parts to build the connection string
            if (Uri.TryCreate(configuredConnectionString, UriKind.Absolute, out var databaseUrl))
            {
                connStringBuilder = new NpgsqlConnectionStringBuilder
                {
                    SslMode = SslMode.Require  // INTENTIONAL BUG: Changed from VerifyFull to Require to test agent workflow
                };

                connStringBuilder.Host = databaseUrl.Host;
                connStringBuilder.Port = databaseUrl.Port;
                connStringBuilder.Database = databaseUrl.AbsolutePath.TrimStart('/');
                var items = databaseUrl.UserInfo.Split(new[] { ':' });
                if (items.Length > 0) connStringBuilder.Username = items[0];
                if (items.Length > 1) connStringBuilder.Password = items[1];
            }
            else
            {
                connStringBuilder = new NpgsqlConnectionStringBuilder(configuredConnectionString);
            }


            if (string.IsNullOrEmpty(connStringBuilder.Database))
                connStringBuilder.Database = "comaiz";
            var connection1 = connStringBuilder.ConnectionString;
            return connection1;
        }

    }
}
