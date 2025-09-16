#!/bin/bash

echo "🔍 Comaiz Admin Environment Verification"
echo "========================================"
echo ""

# Check .NET SDK version
echo "📦 .NET SDK Version:"
dotnet --version
echo ""

# Check if .NET 9.0 is available
echo "📋 Available .NET SDKs:"
dotnet --list-sdks
echo ""

# Check PostgreSQL connection
echo "🗄️  PostgreSQL Status:"
if pg_isready -h localhost -p 5432 -U postgres > /dev/null 2>&1; then
    echo "✅ PostgreSQL is running and accessible"
else
    echo "❌ PostgreSQL is not accessible"
    echo "   Try: docker-compose -f .devcontainer/docker-compose.yml up -d db"
fi
echo ""

# Check if application is running
echo "🌐 Application Status:"
if curl -k -s https://localhost:7057/swagger > /dev/null 2>&1; then
    echo "✅ Application is running on HTTPS (port 7057)"
elif curl -s http://localhost:5000/swagger > /dev/null 2>&1; then
    echo "✅ Application is running on HTTP (port 5000)"
else
    echo "❌ Application is not running"
    echo "   Try: .devcontainer/start-app.sh"
fi
echo ""

# Check HTTPS certificates
echo "🔐 HTTPS Certificate Status:"
if dotnet dev-certs https --check > /dev/null 2>&1; then
    echo "✅ HTTPS developer certificate is available"
else
    echo "❌ HTTPS developer certificate is missing"
    echo "   Try: dotnet dev-certs https"
fi
echo ""

# Check port forwarding (VS Code specific)
if [ -n "$VSCODE_INJECTION" ]; then
    echo "📡 VS Code Environment:"
    echo "✅ Running in VS Code environment"
    echo "   Check the 'Ports' tab to see forwarded ports"
else
    echo "📡 VS Code Environment:"
    echo "ℹ️  Not running in VS Code (port forwarding may not be available)"
fi
echo ""

# Check logs if they exist
if [ -f "/tmp/app.log" ]; then
    echo "📖 Recent Application Logs:"
    echo "Last 10 lines from /tmp/app.log:"
    tail -n 10 /tmp/app.log
else
    echo "📖 Application Logs:"
    echo "ℹ️  No application logs found at /tmp/app.log"
fi
echo ""

echo "🛠️  Quick Commands:"
echo "   Start app:     .devcontainer/start-app.sh"
echo "   Check logs:    tail -f /tmp/app.log"
echo "   Test DB:       psql -h localhost -p 5432 -U postgres -d comaiz"
echo "   Stop app:      pkill -f comaiz.api"
echo "   Fix HTTPS:     dotnet dev-certs https"
echo "   Create migration: dotnet ef migrations add MigrationName -p comaiz.data -s comaiz.api"
echo ""