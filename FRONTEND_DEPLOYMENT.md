# Frontend Deployment Guide

This document explains how the React frontend is integrated with the .NET backend in the Comaiz Admin application.

## Architecture Overview

The application uses a single Docker container that serves both the backend API and the React frontend:

```
┌─────────────────────────────────────┐
│      Docker Container               │
│                                     │
│  ┌────────────────────────────┐    │
│  │   .NET 9.0 API             │    │
│  │   - Serves at /api/*       │    │
│  │   - JWT Authentication     │    │
│  │   - Entity Controllers     │    │
│  └────────────────────────────┘    │
│                                     │
│  ┌────────────────────────────┐    │
│  │   Static File Middleware   │    │
│  │   - Serves React SPA       │    │
│  │   - From wwwroot/          │    │
│  │   - SPA fallback routing   │    │
│  └────────────────────────────┘    │
└─────────────────────────────────────┘
```

## How It Works

### 1. Docker Build Process

The Dockerfile uses multi-stage builds to:

1. **Backend Build** (dotnet SDK stage):
   - Restores NuGet packages
   - Compiles .NET projects
   - Publishes the API

2. **Frontend Build** (node stage):
   - Installs npm dependencies
   - Builds React production bundle
   - Creates optimized static files

3. **Final Image** (dotnet runtime stage):
   - Copies published API
   - Copies React build to `wwwroot/`
   - Sets up runtime environment

### 2. Request Routing

When a request comes to the application:

1. **API Requests** (`/api/*`):
   - Routed to .NET controllers
   - JWT authentication applied
   - Returns JSON responses

2. **Frontend Requests** (`/*`):
   - First checks for static files in `wwwroot/`
   - If not found, serves `index.html` (SPA fallback)
   - React Router handles client-side routing

### 3. Static File Serving

The .NET backend is configured in `Program.cs` to:

```csharp
app.UseStaticFiles();        // Serve static files from wwwroot
app.MapControllers();        // Map API controllers
app.MapFallbackToFile("index.html");  // SPA fallback routing
```

This setup ensures:
- React assets (JS, CSS, images) are served efficiently
- All unknown routes return the React app
- React Router handles navigation client-side

## Development vs Production

### Development Mode

In development, you run frontend and backend separately to enable hot-reload and faster iteration:

**Backend** (port 7057 for HTTPS, 5057 for HTTP):
```bash
dotnet run --project comaiz.api
```

**Frontend** (port 3000):
```bash
cd frontend
npm start
```

**Important**: The backend includes CORS configuration that allows requests from `http://localhost:3000` when running in Development mode. This is automatically enabled based on the `ASPNETCORE_ENVIRONMENT` variable.

Create a `.env.local` file in the `frontend/` directory:
```env
# Use HTTP if backend runs on port 5057
REACT_APP_API_URL=http://localhost:5057/api

# OR use HTTPS if backend runs on port 7057 (default)
REACT_APP_API_URL=https://localhost:7057/api
```

**Note**: If using HTTPS, you may need to trust the development certificate:
```bash
dotnet dev-certs https --trust
```

### Troubleshooting Local Development

**CORS Errors**: If you see "strict-origin-when-cross-origin" or CORS-related errors:
1. Verify the backend is running in Development mode (check console output)
2. Ensure `.env.local` has the correct `REACT_APP_API_URL`
3. Restart both frontend and backend after configuration changes

**405 Method Not Allowed**: This usually indicates CORS preflight (OPTIONS) request is being rejected:
1. Verify CORS is enabled in `Program.cs` (should be automatic in Development)
2. Check that the frontend URL in `.env.local` matches the backend port
3. Ensure the backend is running before starting the frontend

### Production Mode

In production, both are served from a single container (port 8080/8081):

1. Frontend is built into static files
2. Static files are copied to backend's `wwwroot/`
3. Backend serves both API and frontend
4. Frontend uses `/api` (relative URL) for API calls

## CI/CD Pipeline

The GitHub Actions workflow (`.github/workflows/dotnet.yml`):

1. **Setup Node.js** - Installs Node.js 20
2. **Install Frontend Dependencies** - Runs `npm ci` in frontend/
3. **Build Frontend** - Runs `npm run build` to create production bundle
4. **Setup .NET** - Installs .NET 9.0 SDK
5. **Build Backend** - Compiles .NET projects
6. **Run Tests** - Executes .NET tests
7. **Build Docker Image** - Multi-stage build including frontend
8. **Push to Registry** - Uploads to GitHub Container Registry
9. **Deploy** - Deploys to production server

## Environment Variables

### Backend (.NET)

Required environment variables for the backend:

- `ConnectionStrings__PostgresSQL` - Database connection string
- `JwtSettings__SecretKey` - JWT secret (min 32 chars)
- `JwtSettings__Issuer` - JWT issuer
- `JwtSettings__Audience` - JWT audience

### Frontend (React)

Development only:
- `REACT_APP_API_URL` - API endpoint (defaults to `/api` in production)

## Deployment

### Local Docker Build

To build and run the Docker image locally:

```bash
# Build the image
docker build -t comaiz-admin .

# Run the container
docker run -p 8080:8080 \
  -e ConnectionStrings__PostgresSQL="<your-connection-string>" \
  -e JwtSettings__SecretKey="<your-secret-key>" \
  -e JwtSettings__Issuer="comaiz-api" \
  -e JwtSettings__Audience="comaiz-client" \
  comaiz-admin
```

Access the application at `http://localhost:8080`

### Production Deployment

The application is automatically deployed via GitHub Actions when changes are pushed to the master branch.

The deployment:
1. Builds a new Docker image with both frontend and backend
2. Pushes to GitHub Container Registry
3. SSH to production server
4. Pulls the new image
5. Stops and removes old container
6. Starts new container with environment variables

## Troubleshooting

### Frontend Not Loading

If the frontend doesn't load:

1. Check if `wwwroot/` exists in the container:
   ```bash
   docker exec <container-id> ls -la /app/wwwroot
   ```

2. Verify static files middleware is enabled in `Program.cs`

3. Check browser console for errors

### API Calls Failing

If API calls fail:

1. Check browser Network tab for the actual request URL
2. Verify the API is responding at `/api/*`
3. Check CORS settings if needed
4. Verify JWT token is being sent in headers

### Build Failures

If Docker build fails:

1. **Frontend build errors**: Check `frontend/` directory is copied correctly
2. **npm ci fails**: Verify `package-lock.json` is committed
3. **Node version issues**: Dockerfile uses Node 20, ensure compatibility

## File Structure

```
/
├── comaiz.api/
│   ├── Program.cs              # Configures static files and SPA fallback
│   └── wwwroot/                # Created during Docker build
│       └── (React build files)
├── frontend/
│   ├── src/                    # React source code
│   ├── public/                 # Static assets
│   ├── build/                  # Production build (gitignored)
│   └── package.json
├── Dockerfile                  # Multi-stage build
├── .dockerignore              # Excludes node_modules, build artifacts
└── .github/workflows/
    └── dotnet.yml             # CI/CD pipeline
```

## Security Considerations

1. **JWT Tokens**: Stored in localStorage, automatically attached to API requests
2. **HTTPS**: Production should use HTTPS (port 8081)
3. **CORS**: Not needed since frontend and backend share the same origin
4. **Dependencies**: 
   - Frontend dependencies checked with `npm audit`
   - Backend dependencies managed with NuGet
   - Regular updates recommended

## Monitoring and Logs

To view logs from the running container:

```bash
# View all logs
docker logs <container-name>

# Follow logs in real-time
docker logs -f <container-name>

# View only .NET application logs
docker logs <container-name> 2>&1 | grep -i "comaiz"
```

## Future Enhancements

Potential improvements:

1. **Separate Containers**: Split frontend and backend into separate containers
2. **CDN**: Serve frontend static assets from a CDN
3. **Build Tool**: Migrate from create-react-app to Vite for faster builds
4. **Server-Side Rendering**: Implement SSR for better SEO
5. **Progressive Web App**: Add PWA features for offline support

## Support

For issues related to deployment:
- Check GitHub Actions workflow runs
- Review Docker build logs
- Consult the main [README.md](../README.md) for general setup
- See [frontend/README.md](../frontend/README.md) for frontend-specific details
