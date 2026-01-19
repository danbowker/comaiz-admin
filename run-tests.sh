#!/bin/bash

# Test orchestration script for Copilot PR development
# This script starts the backend, frontend, and runs E2E tests

set -e  # Exit on error

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Configuration
BACKEND_PORT=7057
FRONTEND_PORT=3000
BACKEND_DIR="comaiz.api"
FRONTEND_DIR="frontend"
BACKEND_LOG="backend.log"
FRONTEND_LOG="frontend.log"
BACKEND_PID=""
FRONTEND_PID=""

# Function to print colored messages
print_info() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

# Function to cleanup processes on exit
cleanup() {
    print_info "Cleaning up processes..."
    
    if [ ! -z "$BACKEND_PID" ]; then
        print_info "Stopping backend (PID: $BACKEND_PID)..."
        kill $BACKEND_PID 2>/dev/null || true
        wait $BACKEND_PID 2>/dev/null || true
    fi
    
    if [ ! -z "$FRONTEND_PID" ]; then
        print_info "Stopping frontend (PID: $FRONTEND_PID)..."
        kill $FRONTEND_PID 2>/dev/null || true
        wait $FRONTEND_PID 2>/dev/null || true
    fi
    
    # Also kill any remaining dotnet or npm processes on these ports
    lsof -ti:$BACKEND_PORT | xargs kill -9 2>/dev/null || true
    lsof -ti:$FRONTEND_PORT | xargs kill -9 2>/dev/null || true
    
    print_success "Cleanup complete"
}

# Set trap to cleanup on script exit
trap cleanup EXIT INT TERM

# Function to check if a port is in use
is_port_in_use() {
    lsof -i:$1 >/dev/null 2>&1
}

# Function to wait for a service to be ready
wait_for_service() {
    local url=$1
    local name=$2
    local max_attempts=60
    local attempt=0
    
    print_info "Waiting for $name to be ready at $url..."
    
    while [ $attempt -lt $max_attempts ]; do
        if curl -k -s -f -o /dev/null "$url"; then
            print_success "$name is ready!"
            return 0
        fi
        
        attempt=$((attempt + 1))
        echo -n "."
        sleep 2
    done
    
    echo ""
    print_error "$name did not become ready in time"
    return 1
}

# Function to check for backend errors
check_backend_errors() {
    if [ -f "$BACKEND_LOG" ]; then
        if grep -i "error\|exception\|fail" "$BACKEND_LOG" | grep -v "Microsoft.EntityFrameworkCore" | grep -v "EnsureCreated" > /tmp/backend_errors.txt; then
            print_warning "Backend errors detected:"
            cat /tmp/backend_errors.txt
            return 1
        fi
    fi
    return 0
}

# Function to check for frontend errors
check_frontend_errors() {
    if [ -f "$FRONTEND_LOG" ]; then
        if grep -i "error\|failed\|compilation failed" "$FRONTEND_LOG" | grep -v "warning" > /tmp/frontend_errors.txt; then
            print_warning "Frontend errors detected:"
            cat /tmp/frontend_errors.txt
            return 1
        fi
    fi
    return 0
}

# Main script starts here
print_info "Starting test orchestration..."
print_info "========================================"

# Step 1: Check prerequisites
print_info "Checking prerequisites..."

if ! command -v dotnet &> /dev/null; then
    print_error ".NET SDK is not installed"
    exit 1
fi

if ! command -v npm &> /dev/null; then
    print_error "npm is not installed"
    exit 1
fi

if ! command -v node &> /dev/null; then
    print_error "Node.js is not installed"
    exit 1
fi

print_success "All prerequisites are installed"

# Step 2: Check if ports are already in use
if is_port_in_use $BACKEND_PORT; then
    print_warning "Backend port $BACKEND_PORT is already in use. Stopping existing process..."
    lsof -ti:$BACKEND_PORT | xargs kill -9 2>/dev/null || true
    sleep 2
fi

if is_port_in_use $FRONTEND_PORT; then
    print_warning "Frontend port $FRONTEND_PORT is already in use. Stopping existing process..."
    lsof -ti:$FRONTEND_PORT | xargs kill -9 2>/dev/null || true
    sleep 2
fi

# Step 3: Build backend
print_info "Building backend..."
dotnet build --no-restore || {
    print_error "Backend build failed"
    exit 1
}
print_success "Backend built successfully"

# Step 4: Start backend
print_info "Starting backend on port $BACKEND_PORT..."
cd $BACKEND_DIR
ASPNETCORE_ENVIRONMENT=Development dotnet run --no-build > ../$BACKEND_LOG 2>&1 &
BACKEND_PID=$!
cd ..
print_info "Backend started with PID: $BACKEND_PID"

# Wait for backend to be ready
if ! wait_for_service "https://localhost:$BACKEND_PORT/health" "Backend"; then
    print_error "Backend failed to start. Checking logs..."
    tail -50 $BACKEND_LOG
    exit 1
fi

# Check for backend errors
if ! check_backend_errors; then
    print_warning "Backend started but has errors. Continuing anyway..."
fi

# Step 5: Install frontend dependencies (if needed)
print_info "Checking frontend dependencies..."
cd $FRONTEND_DIR
if [ ! -d "node_modules" ] || [ ! -d "node_modules/@playwright" ]; then
    print_info "Installing frontend dependencies..."
    npm install
fi
print_success "Frontend dependencies are ready"

# Step 6: Create frontend .env.local if it doesn't exist
if [ ! -f ".env.local" ]; then
    print_info "Creating .env.local for frontend..."
    echo "REACT_APP_API_URL=https://localhost:$BACKEND_PORT/api" > .env.local
    print_success ".env.local created"
fi

# Step 7: Start frontend
print_info "Starting frontend on port $FRONTEND_PORT..."
npm start > ../$FRONTEND_LOG 2>&1 &
FRONTEND_PID=$!
cd ..
print_info "Frontend started with PID: $FRONTEND_PID"

# Wait for frontend to be ready
if ! wait_for_service "http://localhost:$FRONTEND_PORT" "Frontend"; then
    print_error "Frontend failed to start. Checking logs..."
    tail -50 $FRONTEND_LOG
    exit 1
fi

# Check for frontend errors
if ! check_frontend_errors; then
    print_warning "Frontend started but has errors. Continuing anyway..."
fi

# Step 8: Run E2E tests
print_info "========================================"
print_success "Both services are running!"
print_info "Backend: https://localhost:$BACKEND_PORT"
print_info "Frontend: http://localhost:$FRONTEND_PORT"
print_info "========================================"
print_info "Running E2E tests..."

cd $FRONTEND_DIR
mkdir -p screenshots test-results

# Run Playwright tests
if npm run test:e2e; then
    print_success "E2E tests passed!"
    E2E_EXIT_CODE=0
else
    print_error "E2E tests failed!"
    E2E_EXIT_CODE=1
fi

cd ..

# Step 9: Display results
print_info "========================================"
print_info "Test Results Summary"
print_info "========================================"

if [ $E2E_EXIT_CODE -eq 0 ]; then
    print_success "✓ E2E tests: PASSED"
else
    print_error "✗ E2E tests: FAILED"
fi

# Check for screenshots
SCREENSHOT_COUNT=$(ls -1 $FRONTEND_DIR/screenshots/*.png 2>/dev/null | wc -l)
print_info "Screenshots captured: $SCREENSHOT_COUNT"
if [ $SCREENSHOT_COUNT -gt 0 ]; then
    print_info "Screenshots are available in: $FRONTEND_DIR/screenshots/"
fi

# Display backend status
print_info "========================================"
print_info "Service Status"
print_info "========================================"
if ps -p $BACKEND_PID > /dev/null 2>&1; then
    print_success "Backend: RUNNING"
else
    print_error "Backend: STOPPED"
fi

if ps -p $FRONTEND_PID > /dev/null 2>&1; then
    print_success "Frontend: RUNNING"
else
    print_error "Frontend: STOPPED"
fi

# Logs location
print_info "========================================"
print_info "Logs are available at:"
print_info "  Backend:  $BACKEND_LOG"
print_info "  Frontend: $FRONTEND_LOG"
print_info "========================================"

# Exit with the test exit code
exit $E2E_EXIT_CODE
