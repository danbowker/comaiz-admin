using comaiz.data;
using comaiz.data.Models;
using comaiz.data.Services;
using comaiz.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Works with appsettings.json or appsettings.[Environment].json or
// with dotnet user-secrets set "ConnectionStrings:PostgresSQL" or with environment variable
var connectionString = builder.Configuration.GetConnectionString("PostgresSQL");
Console.WriteLine($"Connection string: {connectionString}");

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<ComaizContext>(options =>
{
    options.UseNpgsql(connectionString.GetNpgsqlConnectionString());
});

// Configure Identity
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;

    // User settings
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ComaizContext>()
.AddDefaultTokenProviders();

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

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
