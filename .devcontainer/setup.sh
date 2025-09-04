#!/bin/bash

echo "🚀 Setting up Codespace for Comaiz Admin..."

# Ensure tools are available
export PATH="$PATH:/root/.dotnet/tools"

# Restore dependencies
echo "📦 Restoring .NET dependencies..."
dotnet restore

# Restore dotnet tools
echo "🔧 Restoring .NET tools..."
dotnet tool restore

# Install EF tools globally if not already installed
if ! command -v dotnet-ef &> /dev/null; then
    echo "📊 Installing Entity Framework tools..."
    dotnet tool install --global dotnet-ef
fi

echo "✅ Codespace setup complete!"
echo ""
echo "🔨 To start the application manually:"
echo "   dotnet run --project comaiz.api --launch-profile swagger"
echo ""
echo "📝 The application will be available at:"
echo "   - Swagger UI: https://localhost:7057/swagger"
echo "   - API Base:   https://localhost:7057"
echo ""
echo "📊 PostgreSQL Database:"
echo "   - Host: localhost:5432"
echo "   - Database: comaiz"
echo "   - Username: postgres"
echo "   - Password: devpassword"