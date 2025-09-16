# GitHub Codespaces Development Guide

This repository is configured with a full development environment using GitHub Codespaces, allowing you to start developing immediately without any local setup.

## Getting Started

### 1. Launch Codespace

1. Go to the repository on GitHub
2. Click the green **"Code"** button
3. Select the **"Codespaces"** tab
4. Click **"Create codespace on main"**

The codespace will automatically set up everything you need:
- .NET 9.0 SDK
- PostgreSQL database
- Entity Framework tools
- VS Code with C# extensions

### 2. Automatic Setup Process

When your Codespace starts, it will automatically:

1. **Install Dependencies**: Restore NuGet packages and .NET tools
2. **Start PostgreSQL**: Launch a development database server
3. **Run Migrations**: Create database tables using Entity Framework migrations
4. **Start the API**: Launch the application with Swagger UI
5. **Open Swagger**: Automatically open the Swagger UI in your browser

### 3. Accessing the Application

After setup (usually 2-3 minutes), the application will be available at:

- **Swagger UI**: `https://localhost:7057/swagger` (opens automatically)
- **API Base URL**: `https://localhost:7057`
- **HTTP Version**: `http://localhost:5000` (if needed)

## Development Environment Details

### Pre-configured Services

| Service | Details |
|---------|---------|
| **PostgreSQL** | Host: `localhost`, Port: `5432`<br>Database: `comaiz`<br>Username: `postgres`, Password: `devpassword` |
| **API Server** | HTTPS: `https://localhost:7057`<br>HTTP: `http://localhost:5000` |
| **Swagger UI** | Available at `/swagger` endpoint |

### Environment Variables

The following environment variables are pre-configured for development:

```bash
ASPNETCORE_ENVIRONMENT=Development
ConnectionStrings__PostgresSQL=Host=localhost;Port=5432;Username=postgres;Password=devpassword;Database=comaiz
Jwt__Authority=https://accounts.google.com
Jwt__Audience=development-client-id
```

### VS Code Extensions

The following extensions are automatically installed:

- **C# Dev Kit** - Full C# development support
- **REST Client** - For testing API endpoints
- **JSON** - Enhanced JSON editing

## Development Workflow

### Starting the Application

The application starts automatically, but if you need to restart it:

```bash
# Stop the current application
pkill -f comaiz.api

# Start the application
dotnet run --project comaiz.api --launch-profile swagger
```

### Database Operations

```bash
# Apply database migrations
dotnet ef database update -p comaiz.data -s comaiz.api

# Create a new migration
dotnet ef migrations add MigrationName -p comaiz.data -s comaiz.api

# View database logs
docker logs $(docker ps -q --filter ancestor=postgres:16)
```

### Testing the API

1. **Using Swagger UI**: Navigate to `https://localhost:7057/swagger`
2. **Using REST Client**: Create `.http` files and use the REST Client extension
3. **Using curl**: Test endpoints directly from the terminal

Example API test:
```bash
curl -X GET "https://localhost:7057/api/your-endpoint" -k
```

### Authentication in Development

The development environment uses dummy JWT settings:
- **Authority**: `https://accounts.google.com`
- **Audience**: `development-client-id`

For actual authentication testing, you'll need to:
1. Set up OAuth2 with Google or another provider
2. Update the JWT settings in the devcontainer configuration
3. Obtain a valid JWT token for testing

## Troubleshooting

### Common Issues and Solutions

#### Application Not Starting Automatically

If the application doesn't start automatically when the Codespace loads:

1. **Check startup logs**:
   ```bash
   tail -f /tmp/app.log
   ```

2. **Start manually**:
   ```bash
   .devcontainer/start-app.sh
   ```

3. **Check process status**:
   ```bash
   ps aux | grep dotnet
   ```

#### Swagger UI Not Accessible

If you can't access Swagger UI through the forwarded ports:

1. **Verify application is running**:
   ```bash
   curl -k -s https://localhost:7057/swagger
   # OR
   curl -s http://localhost:5000/swagger
   ```

2. **Check port forwarding in VS Code**:
   - Open the **Ports** tab in the bottom panel
   - Ensure port 7057 is forwarded and set to "Public"
   - Try accessing via the forwarded URL shown in the Ports tab

3. **Restart the application**:
   ```bash
   pkill -f comaiz.api
   .devcontainer/start-app.sh
   ```

#### Database Connection Problems

If you see database connection errors:

1. **Verify PostgreSQL is running**:
   ```bash
   pg_isready -h localhost -p 5432 -U postgres
   ```

2. **Check database service**:
   ```bash
   docker-compose -f .devcontainer/docker-compose.yml ps
   ```

3. **Restart database**:
   ```bash
   docker-compose -f .devcontainer/docker-compose.yml restart db
   ```

4. **Test connection manually**:
   ```bash
   psql -h localhost -p 5432 -U postgres -d comaiz
   # Password: devpassword
   ```

#### Application Won't Start

Check the application logs:
```bash
tail -f /tmp/app.log
```

### Database Connection Issues

Verify PostgreSQL is running:
```bash
pg_isready -h localhost -p 5432 -U postgres
```

### Port Forwarding Issues

Check which ports are forwarded in VS Code:
- Open the **Ports** tab in the terminal panel
- Ensure ports 5000, 7057, and 5432 are forwarded
- Set port 7057 visibility to "Public" if you need external access

### Rebuilding the Devcontainer

If you need to start fresh:
1. In VS Code, open the Command Palette (`Ctrl+Shift+P` or `Cmd+Shift+P`)
2. Search for "Codespaces: Rebuild Container"
3. Select the command to rebuild with latest configuration

## Contributing

When developing in Codespaces:

1. **Create a branch**: `git checkout -b feature/your-feature`
2. **Make your changes**: Use the full VS Code development environment
3. **Test your changes**: Use Swagger UI or create tests
4. **Commit and push**: Standard git workflow
5. **Create a Pull Request**: Through GitHub's web interface

## Production Deployment

The Codespace environment is for development only. For production deployment:

1. Set up proper OAuth2 credentials
2. Configure production database connection strings
3. Set up GitHub Actions secrets as described in the main README
4. Deploy using the existing GitHub Actions workflow

## Need Help?

- **Application logs**: `/tmp/app.log`
- **Database logs**: `docker logs $(docker ps -q --filter ancestor=postgres:16)`
- **VS Code terminal**: Use the integrated terminal for all commands
- **Port forwarding**: Check the Ports tab in VS Code

The Codespace provides a complete, production-like development environment that requires no local setup!