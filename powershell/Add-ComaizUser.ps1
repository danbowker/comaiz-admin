# Add-ComaizUser.ps1
# Administrative script to create users directly in the database using Entity Framework

<#
.SYNOPSIS
Creates a new user in the Comaiz database with the specified role.

.DESCRIPTION
This administrative script creates users directly in the database using Entity Framework.
It should be run by administrators only and is not exposed via the API.

.PARAMETER Username
The username for the new user

.PARAMETER Email
The email address for the new user

.PARAMETER Password
The password for the new user (must meet complexity requirements)

.PARAMETER Role
The role to assign to the user (Admin or User). Default is User.

.PARAMETER ConnectionString
Optional. Database connection string. If not provided, uses the default from appsettings.

.EXAMPLE
.\Add-ComaizUser.ps1 -Username "newadmin" -Email "admin@company.com" -Password "SecurePass@123" -Role "Admin"

.EXAMPLE
.\Add-ComaizUser.ps1 -Username "newuser" -Email "user@company.com" -Password "UserPass@123"

.NOTES
This script requires:
- .NET 9.0 SDK
- Access to the database
- The comaiz.api and comaiz.data projects
#>

param(
    [Parameter(Mandatory=$true)]
    [string]$Username,
    
    [Parameter(Mandatory=$true)]
    [string]$Email,
    
    [Parameter(Mandatory=$true)]
    [string]$Password,
    
    [Parameter(Mandatory=$false)]
    [ValidateSet("Admin", "User")]
    [string]$Role = "User",
    
    [Parameter(Mandatory=$false)]
    [string]$ConnectionString = $null
)

$ErrorActionPreference = "Stop"

# Get the script directory and project paths
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectRoot = Split-Path -Parent $scriptDir
$apiProject = Join-Path $projectRoot "comaiz.api"

Write-Host "Creating user: $Username with role: $Role" -ForegroundColor Cyan

# Create a temporary C# program to add the user
$tempProgram = @"
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using comaiz.data;
using comaiz.data.Models;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile("appsettings.Development.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

var connectionString = args.Length > 4 && !string.IsNullOrEmpty(args[4]) 
    ? args[4] 
    : configuration.GetConnectionString("PostgresSQL");

if (string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine("Error: No connection string found");
    return 1;
}

var services = new ServiceCollection();
services.AddDbContext<ComaizContext>(options =>
    options.UseNpgsql(connectionString));

services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ComaizContext>()
.AddDefaultTokenProviders();

var serviceProvider = services.BuildServiceProvider();

using var scope = serviceProvider.CreateScope();
var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

var username = args[0];
var email = args[1];
var password = args[2];
var role = args[3];

// Ensure role exists
if (!await roleManager.RoleExistsAsync(role))
{
    var roleResult = await roleManager.CreateAsync(new ApplicationRole { Name = role });
    if (!roleResult.Succeeded)
    {
        Console.WriteLine($"Error creating role: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
        return 1;
    }
}

// Check if user already exists
var existingUser = await userManager.FindByNameAsync(username);
if (existingUser != null)
{
    Console.WriteLine($"Error: User '{username}' already exists");
    return 1;
}

existingUser = await userManager.FindByEmailAsync(email);
if (existingUser != null)
{
    Console.WriteLine($"Error: Email '{email}' is already in use");
    return 1;
}

// Create user
var user = new ApplicationUser
{
    UserName = username,
    Email = email,
    EmailConfirmed = true
};

var result = await userManager.CreateAsync(user, password);
if (!result.Succeeded)
{
    Console.WriteLine($"Error creating user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
    return 1;
}

// Assign role
var roleAssignResult = await userManager.AddToRoleAsync(user, role);
if (!roleAssignResult.Succeeded)
{
    Console.WriteLine($"Error assigning role: {string.Join(", ", roleAssignResult.Errors.Select(e => e.Description))}");
    return 1;
}

Console.WriteLine($"Success: User '{username}' created with role '{role}'");
return 0;
"@

# Write the temporary program
$tempFile = Join-Path $env:TEMP "AddUser_$(Get-Date -Format 'yyyyMMddHHmmss').cs"
$tempProgram | Out-File -FilePath $tempFile -Encoding UTF8

try {
    # Change to the API project directory
    Push-Location $apiProject
    
    # Build and run the temporary program
    Write-Host "Running user creation script..." -ForegroundColor Yellow
    
    $args = @($Username, $Email, $Password, $Role)
    if ($ConnectionString) {
        $args += $ConnectionString
    }
    
    $result = & dotnet script $tempFile @args 2>&1
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host $result -ForegroundColor Green
        Write-Host "`nUser created successfully!" -ForegroundColor Green
        Write-Host "You can now login with:" -ForegroundColor Cyan
        Write-Host "  Username: $Username" -ForegroundColor White
        Write-Host "  Password: [your password]" -ForegroundColor White
    } else {
        Write-Host $result -ForegroundColor Red
        throw "Failed to create user"
    }
}
finally {
    Pop-Location
    
    # Clean up temp file
    if (Test-Path $tempFile) {
        Remove-Item $tempFile -Force
    }
}
