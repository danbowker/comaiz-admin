#!/bin/bash

# Function to check if app is running
check_app() {
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
nohup dotnet run --project comaiz.api --launch-profile swagger --no-build > /tmp/app.log 2>&1 &

# Wait for the app to be ready
echo "Waiting for application to start..."
for i in {1..30}; do
    if check_app; then
        echo "Application is ready!"
        break
    fi
    sleep 2
done

# Output the log to see if there are any issues
echo "Application startup log:"
tail -n 20 /tmp/app.log

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

# Keep showing logs
tail -f /tmp/app.log