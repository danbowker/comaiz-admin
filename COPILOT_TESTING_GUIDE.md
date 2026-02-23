# Quick Reference for Copilot PR Testing

This document provides a quick reference for Copilot when working on PRs in this repository.

## Quick Start

### Run Complete Test Suite
```bash
./run-tests.sh
```

This single command will:
- ✅ Build and start the backend API
- ✅ Start the frontend development server  
- ✅ Wait for both services to be ready
- ✅ Run all E2E tests
- ✅ Capture screenshots of UI states
- ✅ Display results and any errors
- ✅ Clean up processes automatically

### Expected Output

**Success:**
```
[SUCCESS] ✓ E2E tests: PASSED
Screenshots captured: 8
Backend: RUNNING
Frontend: RUNNING
```

**Failure:**
```
[ERROR] ✗ E2E tests: FAILED
[INFO] Logs are available at:
  Backend:  backend.log
  Frontend: frontend.log
```

## What to Check

### Before Making Code Changes
1. Run `./run-tests.sh` to establish baseline
2. Verify all tests pass
3. Note any existing failures (not your responsibility to fix)

### After Making Code Changes
1. Run `./run-tests.sh` again
2. Compare results with baseline
3. If new failures appear, investigate and fix
4. Review captured screenshots for UI changes

### Before Finalizing PR
- ✅ All E2E tests pass locally
- ✅ Screenshots show expected UI state
- ✅ No new errors in backend.log
- ✅ No new errors in frontend.log
- ✅ CI tests pass (check GitHub Actions)

## Common Scenarios

### Scenario: Backend Changes
```bash
# After changing backend code
./run-tests.sh

# If tests fail, check backend log
tail -100 backend.log
```

### Scenario: Frontend Changes
```bash
# After changing frontend code
./run-tests.sh

# If tests fail, check frontend log
tail -100 frontend.log

# Review screenshots
ls -lh frontend/screenshots/
```

### Scenario: New UI Feature
```bash
# Add new E2E tests in frontend/e2e/
# Then run tests
./run-tests.sh

# Verify screenshots capture new UI
ls frontend/screenshots/
```

## Test Files Location

```
frontend/e2e/
├── auth.spec.ts           # Authentication tests
├── navigation.spec.ts     # Navigation tests
├── clients.spec.ts        # Clients CRUD tests
└── utils/
    └── test-helpers.ts    # Helper functions
```

## Manual Testing Commands

### Backend Only
```bash
cd comaiz.api
ASPNETCORE_ENVIRONMENT=Development dotnet run
# Available at: https://localhost:7057
```

### Frontend Only
```bash
cd frontend
npm start
# Available at: http://localhost:3000
```

### E2E Tests Only (requires services running)
```bash
cd frontend
npm run test:e2e          # Headless mode
npm run test:e2e:headed   # See browser
npm run test:e2e:ui       # Interactive mode
npm run test:e2e:debug    # Debug mode
```

## Troubleshooting

### Port Already in Use
```bash
# Kill processes on ports
lsof -ti:7057 | xargs kill -9  # Backend
lsof -ti:3000 | xargs kill -9  # Frontend
```

### Database Issues
```bash
# Check if PostgreSQL is running
docker ps | grep postgres

# Start PostgreSQL if needed
docker run -d --name test-postgres \
  -e POSTGRES_PASSWORD=password \
  -e POSTGRES_DB=comaiz \
  -p 5432:5432 postgres:16-alpine
```

### Tests Hanging
- Press Ctrl+C to stop
- Run: `pkill -f "dotnet run"`
- Run: `pkill -f "npm start"`
- Then try again

### Clean Start
```bash
# Stop all processes
pkill -f "dotnet run"
pkill -f "npm start"

# Clear ports
lsof -ti:7057 | xargs kill -9
lsof -ti:3000 | xargs kill -9

# Run tests again
./run-tests.sh
```

## CI/CD

### GitHub Actions Workflow
- Triggers on: PR, push to master, manual dispatch
- Location: `.github/workflows/dotnet.yml`
- Runs: Unit tests → Integration tests → E2E tests
- Artifacts: Screenshots and test reports (retained 30 days)

### Viewing CI Results
1. Go to **Actions** tab on GitHub
2. Click on the workflow run
3. Check test results in logs
4. Download artifacts:
   - `playwright-report`: HTML test report
   - `e2e-screenshots`: All captured screenshots

### CI Failure Investigation
```bash
# Check the workflow logs in GitHub Actions
# Look for these sections:
# - "Run E2E tests" - shows test execution
# - Upload artifacts - shows what was captured

# Common CI failures:
# - Service startup timeout
# - Test timeout (network issues)
# - Screenshot comparison mismatches
```

## Screenshot Locations

After running tests:
- Local: `frontend/screenshots/*.png`
- CI: Download `e2e-screenshots` artifact from GitHub Actions

## Test Credentials

```typescript
// Admin user
username: 'admin'
password: 'Admin@123'

// Test user
username: 'testuser'
password: 'Test@123'
```

## Quick Commands Cheat Sheet

```bash
# Run everything
./run-tests.sh

# Check backend health
curl -k https://localhost:7057/health

# Check frontend
curl http://localhost:3000

# View recent logs
tail -50 backend.log
tail -50 frontend.log

# Run specific E2E test
cd frontend && npx playwright test e2e/auth.spec.ts

# Open test report
cd frontend && npx playwright show-report

# Clean up everything
docker stop test-postgres && docker rm test-postgres
rm -f backend.log frontend.log
rm -rf frontend/screenshots/*
```

## Performance Expectations

- **Backend startup**: 5-10 seconds
- **Frontend startup**: 10-20 seconds  
- **E2E test suite**: 30-60 seconds
- **Total test run**: 2-5 minutes

## Additional Resources

- Full testing guide: [TESTING.md](TESTING.md)
- Integration tests: [INTEGRATION_TESTS.md](INTEGRATION_TESTS.md)
- Local development: [LOCAL_DEVELOPMENT.md](LOCAL_DEVELOPMENT.md)
- Main README: [README.md](README.md)

## Success Criteria

✅ **Test suite passes**
✅ **No new errors in logs**
✅ **Screenshots show expected UI**
✅ **CI workflow succeeds**
✅ **No regression in existing functionality**

When all criteria are met, the PR is ready to merge! 🎉
