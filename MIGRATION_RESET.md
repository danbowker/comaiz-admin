# EF Core Migration Reset

This document explains the migration reset performed to consolidate all previous migrations into a single `InitialCreate` migration.

## What Changed

### Migrations
- **Removed**: All 17 previous migrations dating from 2024-02-13 to 2025-11-05
- **Created**: Single new `InitialCreate` migration (20251116172538_InitialCreate.cs)

### Database Seeding
The seeding architecture has been rationalized:

1. **Production/All Deployments** (`DatabaseSeeder.SeedDefaultRoles()`)
   - Creates default roles: `Admin` and `User`
   - Called automatically on application startup
   - No users are created by default

2. **Integration Tests** (`TestDataSeeder`)
   - Creates roles: `Admin` and `User`
   - Creates test user: `testuser@test.com` with password `Test@123`
   - Seeds test data (clients, contracts, workers, etc.)
   - Only used in test environment

3. **Manual User Creation**
   - Use PowerShell script: `Add-ComaizUser.ps1`
   - Or command-line: `dotnet run --project comaiz.api -- --create-user <username> <email> <password> [role]`
   - Or `DatabaseSeeder.CreateUser()` method programmatically

## Resetting Existing Databases

If you have an existing database that needs to be reset, follow these steps:

### Option 1: Using SQL Script (Recommended)
```bash
# Connect to your PostgreSQL database and run:
psql -U postgres -d your_database_name -f reset-database.sql

# Then apply the new migration:
dotnet ef database update --project comaiz.data --startup-project comaiz.api
```

### Option 2: Drop and Recreate Database
```bash
# Drop the entire database
dropdb your_database_name

# Create a new database
createdb your_database_name

# Apply the new migration
dotnet ef database update --project comaiz.data --startup-project comaiz.api
```

### Option 3: Using dotnet ef (Destructive)
```bash
# This will drop the database and recreate it
dotnet ef database drop --project comaiz.data --startup-project comaiz.api --force
dotnet ef database update --project comaiz.data --startup-project comaiz.api
```

## After Reset

After resetting the database:

1. **Roles are created automatically** when the application starts
2. **Create users manually** using one of these methods:
   - PowerShell: `.\powershell\Add-ComaizUser.ps1`
   - Command-line: `dotnet run --project comaiz.api -- --create-user admin admin@example.com SecurePass123! Admin`

## New Database Setup

For completely new databases:

```bash
# Apply the migration
dotnet ef database update --project comaiz.data --startup-project comaiz.api

# Start the application - roles will be seeded automatically
dotnet run --project comaiz.api

# Create users as needed
dotnet run --project comaiz.api -- --create-user admin admin@example.com SecurePass123! Admin
```

## Migration History

### Previous Migrations (Removed)
- 20240213201950_InitialCreate
- 20251007185959_dotnet9
- 20251019231904_AddIdentity
- 20251026162623_ReplaceWorkerWithApplicationUser
- 20251029173321_IntroduceTaskEntityRefactoring
- 20251030094246_AddContractToTask
- 20251030161041_AddContractRateToTask
- 20251104194338_AddUserToContractRate
- 20251105152356_AddTaskContractRateCollection

### Current Migration
- 20251116172538_InitialCreate - Comprehensive initial database schema

## Database Schema

The `InitialCreate` migration creates the following tables:

### ASP.NET Identity Tables
- `AspNetUsers` - User accounts
- `AspNetRoles` - User roles (Admin, User)
- `AspNetUserRoles` - User-to-role assignments
- `AspNetUserClaims`, `AspNetUserLogins`, `AspNetUserTokens` - Identity features
- `AspNetRoleClaims` - Role claims

### Application Tables
- `Client` - Client information
- `Contract` - Contracts with clients
- `ContractRate` - Billing rates for contracts
- `Worker` - Worker information
- `Task` - Project tasks
- `TaskContractRate` - Task-to-rate associations
- `WorkRecord` - Time tracking records
- `FixedCost` - Fixed cost items
- `CarJourney` - Travel expense tracking
- `Invoice` - Invoice headers
- `InvoiceItem` - Invoice line items

## Testing

All 101 integration tests pass with the new migration structure.
