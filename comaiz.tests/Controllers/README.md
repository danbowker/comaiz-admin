# Controller Unit Tests

This directory contains comprehensive unit tests for all API controllers in the comaiz.api project.

## Testing Approach

### Technology Stack
- **Testing Framework**: xUnit 2.9.3
- **Database**: Entity Framework Core In-Memory Database 9.0.10
- **Test Runner**: dotnet test CLI

### Key Design Decisions

1. **In-Memory Database Instead of Mocking**
   - We use Entity Framework Core's In-Memory Database provider rather than mocking the DbContext
   - Each test gets a unique database instance (using `Guid.NewGuid()`) to ensure complete isolation
   - This approach provides more realistic tests that validate actual Entity Framework behavior
   - No real database (PostgreSQL) is required for running tests

2. **Test Isolation**
   - Each test method creates its own database context
   - Tests can run in parallel without interfering with each other
   - Database state is clean for every test

3. **Coverage**
   - All 8 controllers are fully tested:
     - ClientsController (8 tests)
     - WorkersController (8 tests)
     - ContractsController (8 tests)
     - InvoicesController (8 tests)
     - FixedCostsController (8 tests)
     - WorkRecordsController (8 tests)
     - ContractRatesController (8 tests)
     - InvoiceItemsController (8 tests)
   - **Total: 64 tests**

4. **Test Scenarios**
   - Success cases for all CRUD operations (Create, Read, Update, Delete)
   - Failure cases (Not Found scenarios)
   - Edge cases (entity doesn't exist, invalid IDs)
   - HTTP status code verification (200 OK, 201 Created, 204 No Content, 404 Not Found)

## Running the Tests

### Run All Tests
```bash
dotnet test
```

### Run Only Controller Tests
```bash
dotnet test --filter "FullyQualifiedName~Controllers"
```

### Run Tests for a Specific Controller
```bash
dotnet test --filter "ClientsControllerTests"
dotnet test --filter "WorkersControllerTests"
# ... etc
```

### Run with Detailed Output
```bash
dotnet test --logger "console;verbosity=detailed"
```

### Run in CI Pipeline
The tests integrate seamlessly with CI/CD pipelines. Simply include:
```bash
dotnet restore
dotnet build
dotnet test
```

## Test Structure

Each controller test file follows this pattern:

```csharp
public class [Controller]Tests
{
    private ComaizContext CreateInMemoryContext()
    {
        // Creates a unique in-memory database for each test
    }

    [Fact]
    public async Task Get[Entities]_ReturnsAll[Entities]()
    {
        // Test: GET all entities returns list
    }

    [Fact]
    public async Task Get[Entity]_WithValidId_Returns[Entity]()
    {
        // Test: GET by ID with valid ID returns entity
    }

    [Fact]
    public async Task Get[Entity]_WithInvalidId_ReturnsNotFound()
    {
        // Test: GET by ID with invalid ID returns 404
    }

    [Fact]
    public async Task Post[Entity]_WithValid[Entity]_ReturnsCreatedAtAction()
    {
        // Test: POST creates entity and returns 201 Created
    }

    [Fact]
    public async Task Put[Entity]_WithValid[Entity]_ReturnsNoContent()
    {
        // Test: PUT updates entity and returns 204 No Content
    }

    [Fact]
    public async Task Put[Entity]_When[Entity]NotExists_ReturnsNotFound()
    {
        // Test: PUT with non-existent ID returns 404
    }

    [Fact]
    public async Task Delete[Entity]_WithValidId_ReturnsNoContent()
    {
        // Test: DELETE removes entity and returns 204 No Content
    }

    [Fact]
    public async Task Delete[Entity]_WithInvalidId_ReturnsNotFound()
    {
        // Test: DELETE with invalid ID returns 404
    }
}
```

## Dependencies

The test project (`comaiz.tests.csproj`) includes:

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="9.0.10" />
<PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="9.0.10" />
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.12.0" />
<PackageReference Include="xunit" Version="2.9.3" />
<PackageReference Include="xunit.runner.visualstudio" Version="3.0.1" />
<PackageReference Include="coverlet.collector" Version="6.0.3" />
```

Project references:
- `comaiz.csproj` - Core business logic
- `comaiz.api.csproj` - API controllers
- `comaiz.data.csproj` - Data models and DbContext

## Benefits of This Approach

1. **Fast Execution**: In-memory database is much faster than real database
2. **No External Dependencies**: Tests don't require a running database server
3. **CI/CD Ready**: Easy to integrate into automated pipelines
4. **Reliable**: Tests are deterministic and repeatable
5. **Maintainable**: Clear structure and naming conventions
6. **Comprehensive**: Full CRUD coverage with success and failure scenarios

## Future Enhancements

Potential areas for expansion:
- Add integration tests with a real database
- Add performance/load tests for endpoints
- Add tests for authentication/authorization if implemented
- Add tests for data validation and model binding
- Add tests for exception handling middleware
