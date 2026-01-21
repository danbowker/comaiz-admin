#!/bin/bash

# Sample script for running tests

print_info() {
    echo "[INFO] $1"
}

print_warning() {
    echo "[WARNING] $1"
}

print_error() {
    echo "[ERROR] $1"
}

print_success() {
    echo "[SUCCESS] $1"
}

# Step 1: Build backend
print_info "Building backend..."
# Build command here
# Simulating build
sleep 1
print_success "Backend built successfully"

# Step 3.5: Reset test database to ensure clean state
print_info "Resetting test database..."
if command -v psql &> /dev/null; then
    print_info "Dropping existing test database (if exists)..."
    psql -h localhost -U postgres -c "DROP DATABASE IF EXISTS comaiz_test;" 2>/dev/null || print_warning "Could not drop database (it may not exist)"
    
    print_info "Creating fresh test database..."
    psql -h localhost -U postgres -c "CREATE DATABASE comaiz_test;" 2>/dev/null || {
        print_error "Failed to create test database"
        exit 1
    }
    print_success "Test database reset complete"
else
    print_warning "psql command not found - skipping database reset"
fi

# Step 4: Start backend
print_info "Starting backend..."
# Start command here
