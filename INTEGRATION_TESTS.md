# Integration Tests

This document describes how to run and understand the integration tests for the Comaiz Admin API.

## Overview

The integration tests validate the API endpoints by:
- Deploying a real PostgreSQL database in a Docker container for each test class
- Initializing the database with the application's schema using Entity Framework migrations
- Seeding test data before each test
- Testing CRUD operations (Create, Read, Update, Delete) on API endpoints
- Cleaning up test data after each test
- Disposing of the database container after all tests in a class complete

## Prerequisites

To run the integration tests, you need:
- **.NET 9.0 SDK** or later
- **Docker** installed and running (required for PostgreSQL test containers)
- Internet connection (to pull the PostgreSQL Docker image on first run)

## Running the Tests

### Run All Tests

To run both unit tests and integration tests:

```bash
dotnet test
```

### Run Only Integration Tests

To run only the integration tests:

```bash
dotnet test --filter "FullyQualifiedName~IntegrationTests"
```

### Run Specific Test Class

To run tests for a specific API controller:

```bash
# For Clients API tests
dotnet test --filter "FullyQualifiedName~ClientsApiIntegrationTests"

# For Workers API tests
dotnet test --filter "FullyQualifiedName~WorkersApiIntegrationTests"
```

### Run with Verbose Output

To see detailed test output:

```bash
dotnet test --verbosity normal
```

## Test Infrastructure

### Architecture

The integration tests use the following key components:

1. **ComaizApiWebApplicationFactory** (`comaiz.tests/IntegrationTests/ComaizApiWebApplicationFactory.cs`)
   - Custom `WebApplicationFactory` that replaces the production database with a test database
   - Manages the lifecycle of a PostgreSQL Docker container using Testcontainers
   - Ensures the database schema is created using Entity Framework migrations

2. **TestDataSeeder** (`comaiz.tests/IntegrationTests/TestDataSeeder.cs`)
   - Provides methods to seed dummy test data into the database
   - Provides methods to clean up all test data
   - Seeds consistent test data before each test to ensure predictable results

3. **Test Classes** (e.g., `ClientsApiIntegrationTests.cs`, `WorkersApiIntegrationTests.cs`)
   - Each test class focuses on one API controller
   - Uses xUnit's `IClassFixture` to share the web application factory across tests in the class
   - Uses xUnit's `IAsyncLifetime` to seed data before each test and clean up after

### Database Lifecycle

1. **Per Test Class**: A PostgreSQL Docker container is created once per test class
2. **Before Each Test**: The database is seeded with consistent test data
3. **During Test**: The test executes API calls against the running application
4. **After Each Test**: All test data is removed from the database
5. **After All Tests in Class**: The PostgreSQL container is stopped and removed

This approach ensures:
- Tests are isolated from each other
- Tests can run in parallel (different test classes use different containers)
- Tests are fast (container is reused within a test class)
- Tests are reliable (clean state before each test)

## Test Data

### Clients
The `TestDataSeeder.SeedTestData()` method creates three sample clients:
- **ACME** - Acme Corporation
- **TECH** - TechVision Inc
- **INNO** - Innovation Labs

### Workers
The `TestDataSeeder.SeedTestData()` method creates three sample workers:
- John Doe
- Jane Smith
- Bob Johnson

## Available Tests

### ClientsApiIntegrationTests

Tests the `/api/clients` endpoints:
- `GetClients_ReturnsSuccessAndAllClients` - Tests GET all clients
- `GetClient_WithValidId_ReturnsClient` - Tests GET single client by ID
- `GetClient_WithInvalidId_ReturnsNotFound` - Tests GET with non-existent ID
- `PostClient_WithValidData_CreatesNewClient` - Tests POST to create a new client
- `PutClient_WithValidData_UpdatesClient` - Tests PUT to update a client
- `DeleteClient_WithValidId_DeletesClient` - Tests DELETE to remove a client
- `DeleteClient_WithInvalidId_ReturnsNotFound` - Tests DELETE with non-existent ID

### WorkersApiIntegrationTests

Tests the `/api/workers` endpoints:
- `GetWorkers_ReturnsSuccessAndAllWorkers` - Tests GET all workers
- `GetWorker_WithValidId_ReturnsWorker` - Tests GET single worker by ID
- `GetWorker_WithInvalidId_ReturnsNotFound` - Tests GET with non-existent ID
- `PostWorker_WithValidData_CreatesNewWorker` - Tests POST to create a new worker
- `PutWorker_WithValidData_UpdatesWorker` - Tests PUT to update a worker
- `DeleteWorker_WithValidId_DeletesWorker` - Tests DELETE to remove a worker
- `DeleteWorker_WithInvalidId_ReturnsNotFound` - Tests DELETE with non-existent ID

## CI/CD Integration

The integration tests are automatically run as part of the GitHub Actions workflow defined in `.github/workflows/dotnet.yml`. The workflow:
1. Checks out the code
2. Sets up .NET 9.0
3. Restores dependencies
4. Builds the solution
5. Runs all tests (including integration tests)

The workflow includes Docker in the build environment, so the PostgreSQL test containers work correctly in CI/CD.

## Adding New Integration Tests

To add integration tests for a new API controller:

1. **Create a new test class** in `comaiz.tests/IntegrationTests/`:
   ```csharp
   public class YourControllerApiIntegrationTests : IClassFixture<ComaizApiWebApplicationFactory>, IAsyncLifetime
   {
       private readonly ComaizApiWebApplicationFactory _factory;
       private readonly HttpClient _client;

       public YourControllerApiIntegrationTests(ComaizApiWebApplicationFactory factory)
       {
           _factory = factory;
           _client = factory.CreateClient();
       }

       public async Task InitializeAsync()
       {
           using var scope = _factory.Services.CreateScope();
           var context = scope.ServiceProvider.GetRequiredService<ComaizContext>();
           TestDataSeeder.SeedTestData(context);
           await Task.CompletedTask;
       }

       public async Task DisposeAsync()
       {
           using var scope = _factory.Services.CreateScope();
           var context = scope.ServiceProvider.GetRequiredService<ComaizContext>();
           TestDataSeeder.ClearTestData(context);
           await Task.CompletedTask;
       }

       [Fact]
       public async Task YourTest_Description()
       {
           // Arrange
           // Act
           var response = await _client.GetAsync("/api/yourcontroller");
           // Assert
           response.EnsureSuccessStatusCode();
       }
   }
   ```

2. **Add test data** to `TestDataSeeder.cs` if needed for the new controller

3. **Run the tests** to verify they work correctly

## Troubleshooting

### Docker Not Available

If you see errors about Docker not being available:
- Ensure Docker Desktop is running (on Windows/Mac)
- Ensure the Docker daemon is running (on Linux)
- Check Docker is in your PATH: `docker --version`

### Port Conflicts

If you see port conflict errors:
- The Testcontainers library automatically selects available ports
- If you still have issues, ensure no other PostgreSQL instances are using standard ports

### Test Timeouts

If tests timeout:
- The first run may take longer as Docker pulls the PostgreSQL image
- Subsequent runs should be faster
- Increase test timeout if needed for slow environments

### Connection String Issues

The test database connection string is automatically generated by the Testcontainers library. If you need to debug connection issues:
- Check the test output for connection-related errors
- Verify Docker containers are running: `docker ps`

## Performance Considerations

- **First Run**: Slower due to Docker image download (~30 seconds)
- **Subsequent Runs**: Faster as the image is cached (~5-10 seconds per test class)
- **Parallel Execution**: Different test classes can run in parallel with separate containers
- **Within Class**: Tests in the same class share a container for better performance

## Package Dependencies

The integration tests use the following NuGet packages:
- `Microsoft.AspNetCore.Mvc.Testing` - Provides `WebApplicationFactory` for in-memory test server
- `Testcontainers.PostgreSql` - Manages PostgreSQL Docker containers for tests
- `xunit` - Test framework
- `Microsoft.EntityFrameworkCore.InMemory` - In-memory database provider (alternative option)

## Best Practices

1. **Keep tests isolated** - Each test should be independent and not rely on other tests
2. **Use consistent test data** - The `TestDataSeeder` ensures predictable test data
3. **Clean up after tests** - Always remove test data to avoid interference between tests
4. **Test all CRUD operations** - Ensure Create, Read, Update, and Delete operations work correctly
5. **Test error cases** - Include tests for invalid inputs and edge cases
6. **Use meaningful test names** - Follow the pattern `MethodName_Scenario_ExpectedBehavior`
