#!/bin/bash

echo "🚀 Setting up Codespace for Comaiz Admin..."

# Verify .NET SDK version
echo "📦 Checking .NET SDK version..."
DOTNET_VERSION=$(dotnet --version)
echo "Current .NET SDK: $DOTNET_VERSION"

if ! echo "$DOTNET_VERSION" | grep -q "^9\." ; then
    echo "⚠️  Warning: Expected .NET 9.0 SDK, but found $DOTNET_VERSION"
    echo "    This may cause build issues. The devcontainer should provide .NET 9.0."
fi

# Ensure tools are available
export PATH="$PATH:/root/.dotnet/tools"

# Restore dependencies
echo "📦 Restoring .NET dependencies..."
if dotnet restore; then
    echo "✅ Dependencies restored successfully"
else
    echo "❌ Failed to restore dependencies"
    echo "    This may be due to .NET SDK version mismatch"
    exit 1
fi

# Restore dotnet tools
echo "🔧 Restoring .NET tools..."
dotnet tool restore

# Install EF tools globally if not already installed
if ! command -v dotnet-ef &> /dev/null; then
    echo "📊 Installing Entity Framework tools..."
    dotnet tool install --global dotnet-ef
else
    echo "✅ Entity Framework tools already available"
fi

# Generate HTTPS developer certificates for the container
echo "🔐 Setting up HTTPS developer certificates..."
if dotnet dev-certs https --check > /dev/null 2>&1; then
    echo "✅ HTTPS developer certificate already exists"
else
    echo "📜 Generating HTTPS developer certificate..."
    dotnet dev-certs https --clean > /dev/null 2>&1 || true
    dotnet dev-certs https --trust > /dev/null 2>&1 || true
    if dotnet dev-certs https > /dev/null 2>&1; then
        echo "✅ HTTPS developer certificate generated successfully"
    else
        echo "⚠️  Warning: Failed to generate HTTPS certificate"
        echo "    The application will only be accessible via HTTP"
    fi
fi

echo "✅ Codespace setup complete!"
echo ""
echo "🔨 To start the application manually:"
echo "   .devcontainer/start-app.sh"
echo ""
echo "🔍 To verify environment:"
echo "   .devcontainer/verify-environment.sh"
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