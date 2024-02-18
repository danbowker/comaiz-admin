using comaiz.data;
using comaiz.data.Services;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Should work with appsettings.json or appsettings.[Environment].json or
// with dotnet user-secrets set "ConnectionStrings:PostgresSQL" or with environment variable
var connectionString = builder.Configuration.GetConnectionString("PostgresSQL");
Console.WriteLine($"Connection string: {connectionString}");

// TODO Extract this to an extension method
NpgsqlConnectionStringBuilder connStringBuilder;

// If the connection string is a URI, parse it and use the parts to build the connection string
if (Uri.TryCreate(connectionString, UriKind.Absolute, out var databaseUrl))
{
    connStringBuilder = new NpgsqlConnectionStringBuilder
    {
        SslMode = SslMode.VerifyFull
    };
    
    connStringBuilder.Host = databaseUrl.Host;
    connStringBuilder.Port = databaseUrl.Port;
    var items = databaseUrl.UserInfo.Split(new[] { ':' });
    if (items.Length > 0) connStringBuilder.Username = items[0];
    if (items.Length > 1) connStringBuilder.Password = items[1];
}
else
{
    connStringBuilder = new NpgsqlConnectionStringBuilder(connectionString);
}


if (string.IsNullOrEmpty(connStringBuilder.Database))
    connStringBuilder.Database = "comaiz";
var connection = connStringBuilder.ConnectionString;

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<ComaizContext>(options =>
{
    options.UseNpgsql(connection);
});

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddSingleton<ExcelAccountsReader>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
else
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<ComaizContext>();
    //DbInitializer.ImportFromExcel(context);
    var excelReader = services.GetRequiredService<ExcelAccountsReader>();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
