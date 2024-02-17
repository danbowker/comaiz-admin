using comaiz.data;
using comaiz.data.Services;
using Microsoft.EntityFrameworkCore;
using Npgsql;

// Required for ExcelDataReader
System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connStringBuilder = new NpgsqlConnectionStringBuilder();
connStringBuilder.SslMode = SslMode.VerifyFull;

// TODO: Use appsettings for connection string except for username and password
// TODO: Use local database for development
// TODO: Use environment variable for production

// To use, setup an app secret with dotnet user-secrets
var databaseUrlEnv = builder.Configuration["CockroachDB"];

var databaseUrl = new Uri(databaseUrlEnv);
connStringBuilder.Host = databaseUrl.Host;
connStringBuilder.Port = databaseUrl.Port;
var items = databaseUrl.UserInfo.Split(new[] { ':' });
if (items.Length > 0) connStringBuilder.Username = items[0];
if (items.Length > 1) connStringBuilder.Password = items[1];

connStringBuilder.Database = "comaiz";

builder.Services.AddRazorPages();
builder.Services.AddDbContext<ComaizContext>(options =>
    options.UseNpgsql(connStringBuilder.ConnectionString));

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
