# Automated Testing Guide for Copilot PR Development

This guide explains how to use the automated testing environment that allows Copilot (and developers) to run and test the application during PR development without manual testing.

## Overview

The testing environment provides:

1. **Automated Service Startup**: Scripts to start both backend and frontend services
2. **Health Checks**: Automated verification that services are running without errors
3. **End-to-End Tests**: Playwright-based E2E tests with screenshot capture
4. **CI Integration**: Automated tests run on every PR and push

## Quick Start

### Running Tests Locally

The easiest way to run the complete test suite is using the orchestration script:

```bash
./run-tests.sh
```

This script will:
1. Build the backend
2. Start the backend API on port 7057
3. Install frontend dependencies
4. Start the frontend on port 3000
5. Wait for both services to be ready
6. Run all E2E tests
7. Capture screenshots
8. Display results and logs
9. Clean up processes

### Running Tests Manually

If you prefer more control, you can run tests manually:

#### 1. Start the Backend

```bash
cd comaiz.api
ASPNETCORE_ENVIRONMENT=Development dotnet run
```

The backend will be available at:
- HTTPS: `https://localhost:7057`
- Swagger: `https://localhost:7057/swagger`

#### 2. Start the Frontend

In a new terminal:

```bash
cd frontend

# Create .env.local if it doesn't exist
echo "REACT_APP_API_URL=https://localhost:7057/api" > .env.local

# Install dependencies (first time only)
npm install

# Start the development server
npm start
```

The frontend will be available at `http://localhost:3000`

#### 3. Run E2E Tests

In a new terminal:

```bash
cd frontend

# Run all E2E tests
npm run test:e2e

# Run tests with UI (interactive mode)
npm run test:e2e:ui

# Run tests in headed mode (see browser)
npm run test:e2e:headed

# Debug a specific test
npm run test:e2e:debug
```

## E2E Test Suite

### Available Tests

#### Authentication Tests (`e2e/auth.spec.ts`)
- **Login Page Display**: Verifies login form is visible
- **Successful Login**: Tests authentication with valid credentials
- **Invalid Credentials**: Verifies error handling
- **Logout**: Tests logout functionality

#### Navigation Tests (`e2e/navigation.spec.ts`)
- **Clients Page**: Tests navigation to Clients
- **Workers Page**: Tests navigation to Workers
- **Contracts Page**: Tests navigation to Contracts
- **Navigation Menu**: Verifies menu functionality

#### Clients CRUD Tests (`e2e/clients.spec.ts`)
- **Display List**: Verifies clients list loads
- **Create Form**: Tests opening create client form
- **View Details**: Tests viewing client details

### Screenshots

Screenshots are automatically captured for:
- Login page
- After successful login
- Each navigation destination
- Error states
- All major UI states

Screenshots are saved in `frontend/screenshots/` and uploaded as artifacts in CI.

## Test Configuration

### Playwright Configuration

The Playwright configuration is in `frontend/playwright.config.ts`. Key settings:

- **Base URL**: `http://localhost:3000` (can be overridden with `PLAYWRIGHT_BASE_URL`)
- **Browser**: Chromium (default)
- **Retries**: 2 retries on CI, 0 locally
- **Artifacts**: Screenshots, videos, and traces on failure

### Environment Variables

For E2E tests:
- `REACT_APP_API_URL`: Backend API URL (default: `https://localhost:7057/api`)
- `PLAYWRIGHT_BASE_URL`: Frontend URL (default: `http://localhost:3000`)
- `CI`: Set to `true` in CI environment

## CI/CD Integration

### GitHub Actions Workflow

E2E tests are automatically run in the GitHub Actions workflow (`.github/workflows/dotnet.yml`) on:
- Every pull request
- Every push to master
- Manual workflow dispatch

The workflow:
1. Builds backend and frontend
2. Runs unit/integration tests
3. Installs Playwright and browsers
4. Starts backend and frontend services
5. Runs E2E tests
6. Uploads test reports and screenshots as artifacts

### Viewing CI Test Results

1. Go to the **Actions** tab in GitHub
2. Click on the workflow run
3. Scroll to the **Artifacts** section
4. Download:
   - `playwright-report`: HTML test report
   - `e2e-screenshots`: All captured screenshots

## Test Helpers

Test helper functions are available in `frontend/e2e/utils/test-helpers.ts`:

### Authentication Helpers
```typescript
import { loginAsAdmin, loginAsTestUser, logout } from './utils/test-helpers';

// Login as admin
await loginAsAdmin(page);

// Login as test user
await loginAsTestUser(page);

// Logout
await logout(page);
```

### Test Credentials
```typescript
import { TEST_USERS } from './utils/test-helpers';

// Admin credentials
TEST_USERS.admin.username // 'admin'
TEST_USERS.admin.password // 'Admin@123'

// Test user credentials
TEST_USERS.testuser.username // 'testuser'
TEST_USERS.testuser.password // 'Test@123'
```

### Utility Functions
```typescript
import { waitForApiCall, takeScreenshot, isVisible } from './utils/test-helpers';

// Wait for specific API call
await waitForApiCall(page, '/api/clients');

// Take a custom screenshot
await takeScreenshot(page, 'my-screenshot');

// Check element visibility
const visible = await isVisible(page, '.my-element');
```

## Writing New Tests

### Test Structure

```typescript
import { test, expect } from '@playwright/test';
import { loginAsAdmin } from './utils/test-helpers';

test.describe('My Feature', () => {
  test.beforeEach(async ({ page }) => {
    // Setup before each test
    await loginAsAdmin(page);
  });

  test('should do something', async ({ page }) => {
    // Navigate
    await page.goto('/my-feature');
    
    // Interact
    await page.click('button');
    
    // Assert
    await expect(page.locator('.result')).toBeVisible();
    
    // Screenshot
    await page.screenshot({ 
      path: 'screenshots/my-feature.png', 
      fullPage: true 
    });
  });
});
```

### Best Practices

1. **Use Helper Functions**: Leverage test helpers for common operations
2. **Capture Screenshots**: Always capture screenshots of important UI states
3. **Use Descriptive Names**: Name tests clearly: `should <action> <expected result>`
4. **Wait Appropriately**: Use `waitForSelector`, `waitForURL`, etc., not `waitForTimeout`
5. **Handle Errors Gracefully**: Tests should skip gracefully if optional features aren't available
6. **Keep Tests Independent**: Each test should work in isolation

## Troubleshooting

### Common Issues

#### Port Already in Use
```bash
# Kill process on port 7057 (backend)
lsof -ti:7057 | xargs kill -9

# Kill process on port 3000 (frontend)
lsof -ti:3000 | xargs kill -9
```

#### Backend Not Starting
```bash
# Check backend logs
tail -f backend.log

# Verify database connection
dotnet ef database update -p comaiz.data -s comaiz.api
```

#### Frontend Not Starting
```bash
# Check frontend logs
tail -f frontend.log

# Clear and reinstall dependencies
cd frontend
rm -rf node_modules package-lock.json
npm install
```

#### Tests Failing on CI but Passing Locally
- Check CI logs for error messages
- Download playwright-report artifact from GitHub Actions
- Verify environment variables are set correctly in CI

#### Playwright Browser Not Installed
```bash
cd frontend
npx playwright install chromium
```

### Debug Mode

To debug a failing test:

```bash
cd frontend

# Run in debug mode
npm run test:e2e:debug

# Or run specific test file
npx playwright test e2e/auth.spec.ts --debug
```

This will:
- Open the Playwright Inspector
- Allow you to step through the test
- Pause on failures
- Show browser actions in real-time

### Viewing Test Reports

After running tests:

```bash
cd frontend
npx playwright show-report
```

This opens an HTML report showing:
- Test results
- Screenshots
- Videos (on failure)
- Traces
- Error messages

## For Copilot

When working on PRs, Copilot should:

1. **Before Making Changes**: Run `./run-tests.sh` to establish baseline
2. **After Making Changes**: Run `./run-tests.sh` again to verify
3. **Check Screenshots**: Review captured screenshots in `frontend/screenshots/`
4. **Review Logs**: Check `backend.log` and `frontend.log` for errors
5. **Verify CI**: Ensure CI tests pass before finalizing PR

### What to Check

- ✅ Backend starts without errors
- ✅ Frontend starts without errors
- ✅ All E2E tests pass
- ✅ Screenshots show expected UI state
- ✅ No console errors in logs
- ✅ CI workflow passes

### When Tests Fail

If tests fail:
1. Check the error message in the test output
2. Review the screenshot captured at failure point
3. Check logs for exceptions or errors
4. Fix the issue and re-run tests
5. Update tests if UI/behavior changed intentionally

## Performance

- **Local Test Run**: ~2-5 minutes
- **CI Test Run**: ~5-10 minutes
- **Screenshot Capture**: ~100-500ms per screenshot
- **Test Parallelization**: Tests run in parallel for speed

## Maintenance

### Updating Dependencies

```bash
cd frontend

# Update Playwright
npm update @playwright/test playwright

# Reinstall browsers
npx playwright install chromium
```

### Adding New Pages

When adding new pages:
1. Create a new test file: `e2e/my-page.spec.ts`
2. Add navigation test in `e2e/navigation.spec.ts`
3. Add CRUD tests if applicable
4. Update this documentation

### Extending Test Coverage

To extend test coverage:
1. Identify untested user flows
2. Create new test files following existing patterns
3. Use test helpers for common operations
4. Capture screenshots of new UI states
5. Run tests locally and in CI

## Resources

- [Playwright Documentation](https://playwright.dev/)
- [Playwright Best Practices](https://playwright.dev/docs/best-practices)
- [Playwright API Reference](https://playwright.dev/docs/api/class-playwright)
- [Repository README](../README.md)
- [Local Development Guide](../LOCAL_DEVELOPMENT.md)
