using Microsoft.EntityFrameworkCore;
using comaiz.Data;
using comaiz.Services;

// Required for ExcelDataReader
System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<ComaizContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("comaizContext") ?? throw new InvalidOperationException("Connection string 'comaizContext' not found.")));

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
    context.Database.EnsureCreated();

    var excelReader = services.GetRequiredService<ExcelAccountsReader>();

    var hta = context.Clients.FirstOrDefault(c => c.ShortName == "HTA");

    foreach (var contract in excelReader.GetContracts())
    {
        contract.Client = hta;
        context.Contracts?.Add(contract);
    }

    context.SaveChanges();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
