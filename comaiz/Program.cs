using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using comaiz.Data;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<comaizContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("comaizContext") ?? throw new InvalidOperationException("Connection string 'comaizContext' not found.")));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

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

app.UseAuthorization();

app.MapRazorPages();

app.Run();
