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
echo "📊 Updating database..."
dotnet ef database update -p comaiz.data -s comaiz.api

# Start the application
echo "🌐 Starting application with Swagger..."
echo ""
echo "📝 Swagger UI will be available at:"
echo "   - HTTPS: https://localhost:7057/swagger (primary)"
echo "   - HTTP:  http://localhost:5000/swagger (fallback)"
echo ""
echo "💡 Press Ctrl+C to stop the application"
echo ""

dotnet run --project comaiz.api --launch-profile swagger