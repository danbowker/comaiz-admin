# Deployment Strategy Changes Summary

## Overview

This document summarizes the changes made to implement a staging and production deployment strategy for the comaiz-admin application.

## Key Changes

### 1. GitHub Actions Workflow (.github/workflows/dotnet.yml)

#### Added Tag-Based Production Trigger
- The workflow triggers on version tag pushes (e.g., `v1.2.3`)
- This enables production deployments automatically when version tags are pushed
- No manual release creation is required

#### Docker Image Tagging
- Images are now tagged with the commit SHA (`${{ github.sha }}`) for traceability
- The `latest` tag is only pushed when a release is created

#### Separate Deployment Jobs
The workflow now has three main jobs:

1. **build**: Builds, tests, and creates Docker image (runs for all events)
2. **deploy-staging**: Deploys to staging environment (runs only on master branch pushes)
3. **deploy-production**: Deploys to production environment (runs only on version tag pushes)

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

#### Deploying to Production
Simple process documented:
1. Create and push a version tag (e.g., `git tag -a v1.0.0 -m "Release 1.0.0"` and `git push origin v1.0.0`)
2. GitHub Actions automatically builds and deploys to production
3. No manual release creation needed

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
2. Push a version tag (e.g., `v1.0.0`) → Should deploy to production

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
- Ensure you're pushing a version tag (e.g., `v1.0.0`)
- Verify the tag matches the pattern `v[0-9]+.[0-9]+.[0-9]+`
- Check that the workflow has the tag trigger configured
- Review GitHub Actions logs for errors

### Container conflicts on server
- If staging and production conflict, check port mappings (8080 vs 8081)
- Ensure container names are different (`comaiz-admin` vs `comaiz-admin-staging`)

### Database connection issues
- Verify connection strings are correct in GitHub Secrets
- Check that database names match (`comaiz_production`, `comaiz_staging`)
- Ensure migrations have been run on both databases
