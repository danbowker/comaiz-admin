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

This will deploy to any server running docker with ssh access. 

### GitHub Actions Secrets

The following secrets are required for the GitHub Actions workflow:

**For Codespaces (Development)**:
- Development environment variables are pre-configured in the devcontainer
- No additional secrets needed for development

**For Production Deployment**:
- `CONNECTION_STRING`: PostgreSQL connection string for production database
- `SSH_PRIVATE_KEY`: Private key for SSH access to deployment server
- `SERVER_IP`: IP address of the deployment server  
- `SERVER_USER`: SSH username for the deployment server

### Production Database

Set up a PostgreSQL database and configure the `CONNECTION_STRING` secret with your production database connection string in the format:
```
Host=your-db-host;Port=5432;Username=your-username;Password=your-password;Database=your-database
```

## Usage

To test from Swagger:

1. Run comaiz.api with the Swagger launch settings
2. Navigate to the Swagger UI (automatically opens in Codespaces)
3. Test the API endpoints directly from the Swagger interface

To test from Powershell

1. Import the Powershell module:
```ps
import-module -name .\powershell\ComaizApi.psm1 -Force
``` 

## Contributing
