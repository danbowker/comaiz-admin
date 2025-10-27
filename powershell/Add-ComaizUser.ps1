# Add-ComaizUser-Simple.ps1
# Simple administrative script to create users using the API project

<#
.SYNOPSIS
Creates a new user in the Comaiz database with the specified role.

.DESCRIPTION
This administrative script creates users by calling the API project with command-line arguments.
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
Optional. Database connection string. If provided, overrides the default connection string.

.EXAMPLE
.\Add-ComaizUser.ps1 -Username "newadmin" -Email "admin@company.com" -Password "SecurePass@123" -Role "Admin"

.EXAMPLE
.\Add-ComaizUser.ps1 -Username "newuser" -Email "user@company.com" -Password "UserPass@123"

.EXAMPLE
.\Add-ComaizUser.ps1 -Username "produser" -Email "prod@company.com" -Password "ProdPass@123" -ConnectionString "Host=prod-server;Port=5432;Username=admin;Password=secret;Database=comaiz"

.NOTES
This script requires:
- .NET 9.0 SDK
- Access to the database
- The comaiz.api project
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

# Validate API project exists
if (!(Test-Path $apiProject)) {
    Write-Error "API project not found at: $apiProject"
    exit 1
}

Write-Host "Creating user: $Username with role: $Role" -ForegroundColor Cyan

try {
    # Change to the API project directory
    Push-Location $apiProject
    
    # Set connection string environment variable if provided
    if ($ConnectionString) {
        $originalConnectionString = $env:ConnectionStrings__PostgresSQL
        $env:ConnectionStrings__PostgresSQL = $ConnectionString
        Write-Host "Using custom connection string" -ForegroundColor Yellow
    }
    
    # Run the API project with user creation arguments
    Write-Host "Running user creation via API project..." -ForegroundColor Yellow
    
    $result = & dotnet run -- "--create-user" $Username $Email $Password $Role
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "`nUser created successfully!" -ForegroundColor Green
        Write-Host "You can now login with:" -ForegroundColor Cyan
        Write-Host "  Username: $Username" -ForegroundColor White
        Write-Host "  Password: [your password]" -ForegroundColor White
    } else {
        throw "Failed to create user (exit code: $LASTEXITCODE)"
    }
}
catch {
    Write-Host "Error creating user: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}
finally {
    # Restore original connection string if it was modified
    if ($ConnectionString) {
        if ($originalConnectionString) {
            $env:ConnectionStrings__PostgresSQL = $originalConnectionString
        } else {
            Remove-Item Env:\ConnectionStrings__PostgresSQL -ErrorAction SilentlyContinue
        }
    }
    Pop-Location
}

Write-Host "`nUser creation completed." -ForegroundColor Green