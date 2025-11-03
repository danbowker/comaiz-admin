[![Board Status](https://danbowker.visualstudio.com/5df69691-7dcf-4009-8166-6e69c4715f85/4057cef7-a02c-4d79-b5f5-028c3e1549ae/_apis/work/boardbadge/72e7a0b8-d5d2-4129-8d9b-2d7e2b9ae9bb)](https://danbowker.visualstudio.com/5df69691-7dcf-4009-8166-6e69c4715f85/_boards/board/t/4057cef7-a02c-4d79-b5f5-028c3e1549ae/Microsoft.RequirementCategory)

# comaiz-admin

An application for managing a small consultancy business.

## 🚀 Quick Start with GitHub Codespaces

**The fastest way to get started is with GitHub Codespaces:**

1. Click the green "Code" button on the repository
2. Select "Codespaces" tab
3. Click "Create codespace on main"

The devcontainer will automatically:
- Set up .NET 9.0 SDK and all dependencies
- Start a PostgreSQL database
- Run database migrations
- Launch the application with Swagger UI
- Open the Swagger UI in your browser at `https://localhost:7057/swagger/index.html`

### Codespace Features

- **Auto-configured Development Environment**: .NET 9.0 SDK, PostgreSQL, Entity Framework tools
- **Pre-configured Database**: PostgreSQL with connection string set up automatically
- **Auto-start Application**: The API launches automatically on Codespace startup
- **Swagger UI**: Accessible at `https://localhost:7057/swagger` for API testing
- **VS Code Extensions**: C# development tools pre-installed

### Environment Variables in Codespaces

The devcontainer includes development-ready environment variables:
- `ConnectionStrings__PostgresSQL`: Pre-configured for local PostgreSQL

**Note**: For production deployment, you'll need to set up proper database connection strings as described in the deployment section below.

📚 **For detailed Codespaces usage instructions, see [CODESPACES.md](CODESPACES.md)**

## Installation (Local Development)

If you prefer local development instead of Codespaces, follow these steps:

Currently, check it out and build it.

It uses [Entity Framework](https://learn.microsoft.com/en-us/ef/) with [PostgresSQL](https://www.postgresql.org/).

This is designed to work with CockroachDB (or any PostgresSQL as a service provider) or with a local PostgresSQL for development.

To work with CockroadDB set up an accout and get a connection string. For local, install PostgresSQL.

To set up the DB:

1. Create a database. Default name is 'comaiz' but you could override in the connection string.
2. Set a "PostgresSQL" connection string in the "comaiz" project. Recommended approaches:
   + For local development, set the "PostgresSQL" connection string in appSettings.Development.json
   + For testing on production, create a user-secret from the command line in the project with:

        ```text
        dotnet user-secrets set ConnectionStrings:PostgresSQL <connection string>
        ```
    + Or set an environment variable

3. Once you have a database and the connection strings setup up, create table by running the following command from terminal in the solution folder:

    ```text
    dotnet ef database update -p comaiz.data -s comaiz
    ```

## Deployment

This application uses GitHub Actions to deploy to both staging and production environments on a server running Docker with SSH access.

### Deployment Strategy

The deployment workflow follows these rules:

1. **Staging Deployment**: Every push to the `master` branch automatically deploys to the staging environment
   - Accessible at: `https://staging.comaiz.co.uk`
   - Uses a separate staging database
   - Runs on port 8081 on the server

2. **Production Deployment**: Only triggered when a GitHub release is created
   - Accessible at: `https://comaiz.co.uk`
   - Uses the production database
   - Runs on port 8080 on the server

### Setting Up GitHub Environments

To enable environment-specific deployments, configure GitHub environments:

1. Go to your repository on GitHub
2. Navigate to **Settings** → **Environments**
3. Create two environments:
   - **staging**: For staging deployments
   - **production**: For production deployments (recommended to add protection rules)

**Recommended Protection Rules for Production**:
- Enable "Required reviewers" to require manual approval before production deployment
- Enable "Wait timer" if you want a delay before deployment

### GitHub Secrets Configuration

Configure the following secrets in your repository (**Settings** → **Secrets and variables** → **Actions**):

**Required for Both Environments**:
- `SSH_PRIVATE_KEY`: Private SSH key for accessing the deployment server
- `SERVER_IP`: IP address or hostname of the deployment server
- `SERVER_USER`: SSH username for the deployment server
- `JWT_AUTHORITY`: JWT authority for authentication (optional, if using external auth)
- `JWT_AUDIENCE`: JWT audience for authentication (optional, if using external auth)

**Environment-Specific Secrets**:
- `STAGING_CONNECTION_STRING`: PostgreSQL connection string for the staging database
- `PRODUCTION_CONNECTION_STRING`: PostgreSQL connection string for the production database

### Database Setup

#### Staging Database

1. Create a PostgreSQL database for staging (e.g., `comaiz_staging`)
2. Configure the `STAGING_CONNECTION_STRING` secret in the format:
   ```
   Host=your-db-host;Port=5432;Username=your-username;Password=your-password;Database=comaiz_staging
   ```
3. Run migrations to create the schema:
   ```bash
   # Set the staging connection string temporarily (use a .env file or directly set for the command)
   # Note: Avoid putting passwords in shell history - use a .env file or prefix with a space if your shell supports it
   export ConnectionStrings__PostgresSQL="Host=your-db-host;Port=5432;Username=your-username;Password=your-password;Database=comaiz_staging"
   
   # Run migrations
   dotnet ef database update -p comaiz.data -s comaiz.api
   
   # Unset the environment variable after use
   unset ConnectionStrings__PostgresSQL
   ```

#### Production Database

1. Create a PostgreSQL database for production (e.g., `comaiz_production`)
2. Configure the `PRODUCTION_CONNECTION_STRING` secret in the format:
   ```
   Host=your-db-host;Port=5432;Username=your-username;Password=your-password;Database=comaiz_production
   ```
3. Run migrations to create the schema:
   ```bash
   # Set the production connection string temporarily (use a .env file or directly set for the command)
   # Note: Avoid putting passwords in shell history - use a .env file or prefix with a space if your shell supports it
   export ConnectionStrings__PostgresSQL="Host=your-db-host;Port=5432;Username=your-username;Password=your-password;Database=comaiz_production"
   
   # Run migrations
   dotnet ef database update -p comaiz.data -s comaiz.api
   
   # Unset the environment variable after use
   unset ConnectionStrings__PostgresSQL
   ```

### Creating a Release (Production Deployment)

To deploy to production, create a GitHub release:

#### Option 1: Using GitHub Web Interface

1. Go to your repository on GitHub
2. Click on **Releases** in the right sidebar
3. Click **Draft a new release**
4. Create a new tag (e.g., `v1.0.0`, `v1.1.0`) following [semantic versioning](https://semver.org/)
5. Enter a release title (e.g., "Version 1.0.0")
6. Add release notes describing the changes
7. Click **Publish release**

The workflow will automatically:
- Build and test the application
- Create a Docker image tagged with the commit SHA and `latest`
- Deploy to the production environment

#### Option 2: Using GitHub CLI

```bash
# Install GitHub CLI if you haven't already
# https://cli.github.com/

# Create and publish a release
gh release create v1.0.0 --title "Version 1.0.0" --notes "Release notes here"
```

#### Option 3: Using Git Tags

```bash
# Create a tag
git tag -a v1.0.0 -m "Version 1.0.0"

# Push the tag
git push origin v1.0.0

# Then create the release from the tag on GitHub
```

### Server Configuration

On your deployment server, ensure:

1. Docker is installed and running
2. The server has ports 8080 (production) and 8081 (staging) available
3. A reverse proxy (like nginx) is configured to route:
   - `comaiz.co.uk` → `localhost:8080` (production)
   - `staging.comaiz.co.uk` → `localhost:8081` (staging)
4. SSL certificates are configured for both domains

### Monitoring Deployments

You can monitor deployment status:

1. Go to **Actions** tab in your GitHub repository
2. View workflow runs for build and deployment status
3. Check the **Environments** section in your repository to see deployment history

## Usage

### Accessing the Application

The application now includes a React-based frontend that provides a full CRUD interface for managing your consultancy business.

**Web Interface:**
- Access the web application at the root URL (e.g., `http://localhost:8080` or your deployed domain)
- Login with default credentials:
  - Username: `admin`, Password: `Admin@123` (or)
  - Username: `testuser`, Password: `Test@123`
- Use the web interface to manage clients, workers, contracts, work records, invoices, and more

**API Access:**
- Swagger UI is available at `/swagger` for API documentation and testing
- See the sections below for programmatic API access

📚 **For detailed frontend setup and usage, see [frontend/README.md](frontend/README.md)**

### Authentication

The API now requires authentication using JWT Bearer tokens. See the [Authentication Guide](AUTHENTICATION.md) for detailed instructions on:
- Getting started with default users
- Using Swagger UI with authentication
- Obtaining and using tokens programmatically
- Creating and managing users
- Security best practices

**Quick Start:**
1. Run the API (it will create default users automatically)
2. Open Swagger UI at `https://localhost:7057/swagger`
3. Use the `/api/auth/login` endpoint with credentials:
   - Username: `admin`, Password: `Admin@123` (or)
   - Username: `testuser`, Password: `Test@123`
4. Copy the returned token
5. Click "Authorize" in Swagger and enter: `Bearer YOUR_TOKEN`
6. Now you can test all API endpoints

### Testing from Swagger

To test from Swagger:

1. Run comaiz.api with the Swagger launch settings
2. Navigate to the Swagger UI (automatically opens in Codespaces)
3. Authenticate using the steps above
4. Test the API endpoints directly from the Swagger interface

### Testing from Powershell

To test from Powershell

1. Import the Powershell module:
```ps
import-module -name .\powershell\ComaizApi.psm1 -Force
```

2. Get an authentication token:
```ps
$token = Get-ComaizToken -BaseUrl "https://localhost:7057" -Username "admin" -Password "Admin@123"
```

3. Use the token to make API calls:
```ps
$clients = Get-Items -BaseUrl "https://localhost:7057" -Token $token -Collection Clients
```

For more examples, see `powershell/Examples.ps1`.

## Testing

The project includes both unit tests and integration tests.

### Running Tests

Run all tests:
```bash
dotnet test
```

Run only integration tests:
```bash
dotnet test --filter "FullyQualifiedName~IntegrationTests"
```

📚 **For detailed information about integration tests, see [INTEGRATION_TESTS.md](INTEGRATION_TESTS.md)**

The integration tests:
- Use PostgreSQL test containers with Docker
- Test API CRUD operations (Create, Read, Update, Delete)
- Initialize and seed a fresh database for each test
- Clean up test data automatically
- Run as part of CI/CD pipeline

## Contributing
