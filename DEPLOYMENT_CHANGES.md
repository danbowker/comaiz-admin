# Deployment Strategy Changes Summary

## Overview

This document summarizes the changes made to implement a staging and production deployment strategy for the comaiz-admin application.

## Key Changes

### 1. GitHub Actions Workflow (.github/workflows/dotnet.yml)

#### Added Release Trigger
- The workflow now triggers on `release` events (specifically when a release is created)
- This enables production deployments only when releases are published

#### Docker Image Tagging
- Images are now tagged with the commit SHA (`${{ github.sha }}`) for traceability
- The `latest` tag is only pushed when a release is created

#### Separate Deployment Jobs
The workflow now has three main jobs:

1. **build**: Builds, tests, and creates Docker image (runs for all events)
2. **deploy-staging**: Deploys to staging environment (runs only on master branch pushes)
3. **deploy-production**: Deploys to production environment (runs only on release creation)

#### Environment Configuration
Both deployment jobs use GitHub Environments:
- **Staging**: `staging` environment with URL `https://staging.comaiz.co.uk`
- **Production**: `production` environment with URL `https://comaiz.co.uk`

#### Container Naming and Ports
- **Staging container**: `comaiz-admin-staging` on port 8081
- **Production container**: `comaiz-admin` on port 8080

### 2. README.md Updates

Added comprehensive deployment documentation including:

#### Deployment Strategy Section
- Clear explanation of when staging and production deployments occur
- URLs for accessing each environment
- Port information

#### Setting Up GitHub Environments
- Step-by-step guide for creating environments in GitHub
- Recommendations for production protection rules (required reviewers, wait timers)

#### GitHub Secrets Configuration
Updated secret requirements:
- **Removed**: `CONNECTION_STRING` (replaced with environment-specific versions)
- **Added**: `STAGING_CONNECTION_STRING` and `PRODUCTION_CONNECTION_STRING`
- Documented all required secrets with clear descriptions

#### Database Setup
- Separate instructions for staging and production databases
- Example connection strings with clear naming conventions (`comaiz_staging`, `comaiz_production`)
- Migration commands for setting up database schema

#### Creating a Release (Production Deployment)
Three options documented:
1. **GitHub Web Interface**: Step-by-step guide with screenshots reference
2. **GitHub CLI**: Command-line approach using `gh` tool
3. **Git Tags**: Traditional git tag approach

#### Server Configuration
- Requirements for Docker installation
- Port availability (8080 for production, 8081 for staging)
- Reverse proxy configuration guidance
- SSL certificate requirements

#### Monitoring Deployments
- How to view deployment status in GitHub Actions
- How to check environment deployment history

## Migration Guide for Existing Setup

To migrate from the old deployment strategy to the new one:

### 1. Create GitHub Environments

1. Go to repository **Settings** → **Environments**
2. Create `staging` environment
3. Create `production` environment
4. (Optional) Add protection rules to production:
   - Required reviewers
   - Wait timer before deployment

### 2. Update GitHub Secrets

1. Go to **Settings** → **Secrets and variables** → **Actions**
2. Rename or recreate secrets:
   - Rename `CONNECTION_STRING` to `PRODUCTION_CONNECTION_STRING`
   - Create new `STAGING_CONNECTION_STRING` secret
3. Verify all other secrets exist:
   - `SSH_PRIVATE_KEY`
   - `SERVER_IP`
   - `SERVER_USER`
   - `JWT_AUTHORITY` (if used)
   - `JWT_AUDIENCE` (if used)

### 3. Set Up Staging Database

1. Create a new PostgreSQL database for staging
2. Run migrations to create the schema:
   ```bash
   export ConnectionStrings__PostgresSQL="Host=your-db-host;Port=5432;Username=your-username;Password=your-password;Database=comaiz_staging"
   dotnet ef database update -p comaiz.data -s comaiz.api
   ```

### 4. Configure Server

On your deployment server:

1. Ensure port 8081 is available for staging
2. Update reverse proxy (nginx) to route:
   - `staging.comaiz.co.uk` → `localhost:8081`
   - `comaiz.co.uk` → `localhost:8080`
3. Configure SSL certificates for `staging.comaiz.co.uk`

### 5. Test the Workflow

1. Push a commit to master → Should deploy to staging only
2. Create a release → Should deploy to production

## Benefits of This Approach

1. **Safer Production Deployments**: Production only updates on explicit release creation
2. **Testing Environment**: Staging environment for testing changes before production
3. **Traceability**: Docker images tagged with commit SHA for better version tracking
4. **Environment Protection**: GitHub Environments support protection rules and approvals
5. **Separate Databases**: Staging and production have isolated databases
6. **Clear Deployment History**: GitHub Environments provide deployment history and status

## Troubleshooting

### Staging deployment not triggering
- Verify you're pushing to the `master` branch
- Check that the workflow file is on the master branch
- Review GitHub Actions logs for errors

### Production deployment not triggering
- Ensure you're creating a release, not just a tag
- Verify the release is published (not a draft)
- Check that the workflow has `release: types: [created]` trigger

### Container conflicts on server
- If staging and production conflict, check port mappings (8080 vs 8081)
- Ensure container names are different (`comaiz-admin` vs `comaiz-admin-staging`)

### Database connection issues
- Verify connection strings are correct in GitHub Secrets
- Check that database names match (`comaiz_production`, `comaiz_staging`)
- Ensure migrations have been run on both databases
