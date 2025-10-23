# Local Development Setup Guide

This guide explains how to set up and run the Comaiz Admin application locally for development.

## Quick Start

### 1. Start the Backend

From the repository root:

```bash
dotnet run --project comaiz.api
```

The backend will start at:
- HTTPS: `https://localhost:7057`
- HTTP: `http://localhost:5057`
- Swagger UI: `https://localhost:7057/swagger`

**Note**: The backend automatically runs in Development mode when using `dotnet run`, which enables CORS for the frontend.

### 2. Configure the Frontend

Create a `.env.local` file in the `frontend/` directory:

```bash
cd frontend
cat > .env.local << EOF
REACT_APP_API_URL=https://localhost:7057/api
EOF
```

**Alternative**: If you prefer HTTP or the backend is running on a different port:
```bash
# For HTTP
REACT_APP_API_URL=http://localhost:5057/api
```

### 3. Install Frontend Dependencies

```bash
npm install
```

### 4. Start the Frontend

```bash
npm start
```

The frontend will start at `http://localhost:3000` and automatically open in your browser.

## Troubleshooting

### CORS Errors (strict-origin-when-cross-origin)

**Symptoms**: Console shows CORS errors, requests fail with "blocked by CORS policy"

**Solution**:
1. Verify the backend is running in Development mode (check console output - should say "Development environment")
2. Ensure `.env.local` exists in the `frontend/` directory with the correct API URL
3. Restart both frontend and backend after creating `.env.local`
4. Clear browser cache and try again

### 405 Method Not Allowed

**Symptoms**: Login fails with 405 error, OPTIONS request fails

**Causes**: This indicates CORS preflight (OPTIONS) requests are being rejected

**Solution**:
1. Verify CORS is enabled in the backend (automatic in Development mode)
2. Check that the backend console shows CORS policy is being applied
3. Ensure the backend URL in `.env.local` matches the actual backend port
4. Make sure the backend started successfully before starting the frontend

### Backend Port Issues

**Symptom**: Backend starts on a different port than expected

**Solution**:
1. Check the backend console output for the actual ports
2. Update `.env.local` with the correct port number
3. Restart the frontend

### HTTPS Certificate Errors

**Symptom**: Browser shows "Your connection is not private" or certificate warnings

**Solution**:
```bash
# Trust the .NET development certificate
dotnet dev-certs https --trust
```

Then restart the browser and try again.

### Backend Not Running in Development Mode

**Symptom**: CORS is not working even after configuration

**Solution**:
```bash
# Explicitly set the environment
ASPNETCORE_ENVIRONMENT=Development dotnet run --project comaiz.api
```

## Verification Steps

After setup, verify everything works:

1. **Backend Health Check**:
   - Open `https://localhost:7057/swagger` in a browser
   - You should see the Swagger UI with all API endpoints

2. **Frontend Access**:
   - Open `http://localhost:3000` in a browser
   - You should see the login page

3. **Login Test**:
   - Username: `admin`
   - Password: `Admin@123`
   - You should be redirected to the dashboard after successful login

4. **API Communication**:
   - After login, navigate to any entity page (e.g., Clients)
   - Open browser DevTools (F12) → Network tab
   - You should see successful API requests to `https://localhost:7057/api/*`

## Development Workflow

### Hot Reload

Both frontend and backend support hot reload:

**Frontend**: Changes to React files automatically reload the browser  
**Backend**: Changes to C# files require restarting the backend (Ctrl+C, then `dotnet run` again)

### Environment Variables

The backend uses these environment variables (set in `appsettings.Development.json` or user secrets):

- `ConnectionStrings__PostgresSQL` - Database connection string
- `JwtSettings__SecretKey` - JWT secret key (min 32 chars)
- `JwtSettings__Issuer` - JWT issuer
- `JwtSettings__Audience` - JWT audience

The frontend uses:

- `REACT_APP_API_URL` - Backend API URL (set in `.env.local`)

### Database Setup

If you haven't set up the database yet:

1. Install PostgreSQL locally or use a cloud instance
2. Create a database named `comaiz`
3. Set the connection string in `appsettings.Development.json` or user secrets:
   ```bash
   dotnet user-secrets set "ConnectionStrings:PostgresSQL" "Host=localhost;Port=5432;Database=comaiz;Username=your_user;Password=your_password"
   ```
4. Run migrations:
   ```bash
   dotnet ef database update -p comaiz.data -s comaiz.api
   ```

## Common Issues

### Port Already in Use

If ports 3000, 5057, or 7057 are already in use:

**Frontend**: Edit `package.json` and change the start script:
```json
"start": "PORT=3001 react-scripts start"
```

**Backend**: Use different ports via environment variables or `launchSettings.json`

### Module Not Found (Frontend)

**Symptom**: `Module not found: Can't resolve 'X'`

**Solution**:
```bash
cd frontend
rm -rf node_modules package-lock.json
npm install
```

### Database Connection Errors

**Symptom**: Backend fails to start with connection errors

**Solution**:
1. Verify PostgreSQL is running
2. Check connection string is correct
3. Ensure database exists
4. Run migrations if needed

## Production Build Testing

To test the production build locally:

```bash
# Build frontend
cd frontend
npm run build

# Build and run Docker image
cd ..
docker build -t comaiz-admin .
docker run -p 8080:8080 \
  -e ConnectionStrings__PostgresSQL="your-connection-string" \
  -e JwtSettings__SecretKey="your-secret-key-min-32-chars" \
  -e JwtSettings__Issuer="comaiz-api" \
  -e JwtSettings__Audience="comaiz-client" \
  comaiz-admin
```

Access at `http://localhost:8080`

## Additional Resources

- **Frontend README**: `frontend/README.md`
- **API Integration**: `API_INTEGRATION.md`
- **Deployment Guide**: `FRONTEND_DEPLOYMENT.md`
- **Authentication Guide**: `AUTHENTICATION.md`

## Getting Help

If you encounter issues not covered here:

1. Check the browser console (F12) for error messages
2. Check the backend console for error logs
3. Verify all configuration files are correct
4. Try restarting both frontend and backend
5. Clear browser cache and localStorage
