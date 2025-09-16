#!/bin/bash

# Function to check if app is running
check_app() {
    # Try HTTPS first (primary), then HTTP as fallback
    curl -k -s https://localhost:7057/swagger > /dev/null 2>&1 || \
    curl -s http://localhost:5000/swagger > /dev/null 2>&1
    return $?
}

# Wait for PostgreSQL to be ready
echo "Waiting for PostgreSQL to be ready..."
until pg_isready -h localhost -p 5432 -U postgres; do
  echo "PostgreSQL is unavailable - sleeping"
  sleep 1
done

echo "PostgreSQL is ready!"

# Navigate to the workspace
cd /workspace

# Build the application first
echo "Building the application..."
dotnet build --no-restore

# Initialize the database (create tables if they don't exist)
echo "Updating database..."
dotnet ef database update -p comaiz.data -s comaiz.api --no-build

echo "Starting the application..."

# Start the application in the background with the swagger profile
echo "📝 Starting dotnet application..."
nohup dotnet run --project comaiz.api --launch-profile swagger --no-build > /tmp/app.log 2>&1 &
APP_PID=$!
echo "🔄 Application started with PID: $APP_PID"

# Give the app a moment to initialize
sleep 3

# Wait for the app to be ready
echo "Waiting for application to start..."
APP_READY=false
for i in {1..30}; do
    echo "Attempt $i/30: Checking if app is ready..."
    if check_app; then
        echo "✅ Application is ready!"
        APP_READY=true
        break
    fi
    if [ $i -eq 10 ] || [ $i -eq 20 ]; then
        echo "🔍 Still waiting... Current log tail:"
        tail -n 5 /tmp/app.log
    fi
    sleep 2
done

# Output the log to see if there are any issues
echo "Application startup log:"
tail -n 20 /tmp/app.log

if [ "$APP_READY" = true ]; then
    echo ""
    echo "🎉 Comaiz Admin is now running!"
    echo ""
    echo "📝 Swagger UI is available at:"
    echo "   - HTTP:  http://localhost:5000/swagger"
    echo "   - HTTPS: https://localhost:7057/swagger"
    echo ""
    echo "📊 Database: PostgreSQL running on localhost:5432"
    echo "📋 Username: postgres, Password: devpassword, Database: comaiz"
    echo ""
    echo "📖 Check /tmp/app.log for application logs"
    echo "💡 To restart the app: pkill -f comaiz.api && dotnet run --project comaiz.api --launch-profile swagger"
    echo ""
    echo "✅ Startup complete! Application is running in the background (PID: $APP_PID)"
else
    echo ""
    echo "❌ Application failed to start properly. Check the logs:"
    tail -n 50 /tmp/app.log
    echo ""
    echo "🔍 Troubleshooting:"
    echo "   - Check if PostgreSQL is running: pg_isready -h localhost -p 5432 -U postgres"
    echo "   - Try manual start: dotnet run --project comaiz.api --launch-profile swagger"
    echo "   - Check database connection: psql -h localhost -p 5432 -U postgres -d comaiz"
fi