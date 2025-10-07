#!/bin/bash

echo "🚀 Starting Comaiz Admin manually..."

# Check if PostgreSQL is running
if ! pg_isready -h localhost -p 5432 -U postgres > /dev/null 2>&1; then
    echo "❌ PostgreSQL is not running. Please start it first:"
    echo "   docker-compose -f .devcontainer/docker-compose.yml up -d db"
    exit 1
fi

echo "✅ PostgreSQL is ready"

# Stop any existing app processes
pkill -f "dotnet.*comaiz.api" || true

# Ensure the database is up to date
echo "📊 Updating database migrations..."
# First, create a new migration if needed to sync EF Core 9.0 model
echo "Creating migration for EF Core 9.0 compatibility..."
dotnet ef migrations add EFCore9Upgrade -p comaiz.data -s comaiz.api || echo "Migration creation failed or not needed"

echo "📊 Updating database..."
dotnet ef database update -p comaiz.data -s comaiz.api

# Generate HTTPS certificates if missing
echo "🔐 Checking HTTPS certificates..."
if ! dotnet dev-certs https --check > /dev/null 2>&1; then
    echo "📜 Generating HTTPS developer certificate..."
    dotnet dev-certs https --clean > /dev/null 2>&1 || true
    if ! dotnet dev-certs https > /dev/null 2>&1; then
        echo "⚠️  Warning: Could not generate HTTPS certificate. Will use HTTP-only mode."
        USE_HTTPS=false
    else
        echo "✅ HTTPS developer certificate generated"
        USE_HTTPS=true
    fi
else
    echo "✅ HTTPS developer certificate is available"
    USE_HTTPS=true
fi

# Start the application
if [ "$USE_HTTPS" = true ]; then
    echo "🌐 Starting application with HTTPS + HTTP (Swagger)..."
    echo ""
    echo "📝 Swagger UI will be available at:"
    echo "   - HTTPS: https://localhost:7057/swagger (primary)"
    echo "   - HTTP:  http://localhost:5000/swagger (fallback)"
    echo ""
    echo "💡 Press Ctrl+C to stop the application"
    echo ""
    
    dotnet run --project comaiz.api --launch-profile swagger
else
    echo "🌐 Starting application with HTTP-only (Swagger)..."
    echo ""
    echo "📝 Swagger UI will be available at:"
    echo "   - HTTP:  http://localhost:5000/swagger"
    echo ""
    echo "💡 Press Ctrl+C to stop the application"
    echo ""
    
    dotnet run --project comaiz.api --launch-profile swagger-http
fi