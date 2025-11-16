-- =====================================================
-- Database Reset Script for Comaiz Admin Application
-- =====================================================
-- 
-- Purpose: This script resets existing databases by dropping all tables
-- and the migration history to prepare for the new InitialCreate migration.
--
-- IMPORTANT: This will delete ALL data in the database!
-- Use this only if you have no production data to preserve.
--
-- After running this script, you must apply the new migration:
--   dotnet ef database update --project comaiz.data --startup-project comaiz.api
--
-- =====================================================

-- Drop all application tables in dependency order
DROP TABLE IF EXISTS "TaskContractRate" CASCADE;
DROP TABLE IF EXISTS "Task" CASCADE;
DROP TABLE IF EXISTS "InvoiceItem" CASCADE;
DROP TABLE IF EXISTS "Invoice" CASCADE;
DROP TABLE IF EXISTS "WorkRecord" CASCADE;
DROP TABLE IF EXISTS "ContractRate" CASCADE;
DROP TABLE IF EXISTS "Contract" CASCADE;
DROP TABLE IF EXISTS "CarJourney" CASCADE;
DROP TABLE IF EXISTS "FixedCost" CASCADE;
DROP TABLE IF EXISTS "Worker" CASCADE;
DROP TABLE IF EXISTS "Client" CASCADE;

-- Drop ASP.NET Identity tables
DROP TABLE IF EXISTS "AspNetUserTokens" CASCADE;
DROP TABLE IF EXISTS "AspNetUserRoles" CASCADE;
DROP TABLE IF EXISTS "AspNetUserLogins" CASCADE;
DROP TABLE IF EXISTS "AspNetUserClaims" CASCADE;
DROP TABLE IF EXISTS "AspNetRoleClaims" CASCADE;
DROP TABLE IF EXISTS "AspNetUsers" CASCADE;
DROP TABLE IF EXISTS "AspNetRoles" CASCADE;

-- Drop Entity Framework migration history table
DROP TABLE IF EXISTS "__EFMigrationsHistory" CASCADE;

-- Verify all tables are dropped
SELECT tablename 
FROM pg_tables 
WHERE schemaname = 'public' 
  AND tablename NOT LIKE 'pg_%'
  AND tablename NOT LIKE 'sql_%';

-- =====================================================
-- Next Steps:
-- =====================================================
-- 1. Run: dotnet ef database update --project comaiz.data --startup-project comaiz.api
-- 2. The new InitialCreate migration will be applied
-- 3. Default roles (Admin and User) will be seeded automatically on application startup
-- 4. Create users as needed using the Add-ComaizUser.ps1 script or the --create-user command
-- =====================================================
