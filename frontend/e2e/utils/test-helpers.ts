import { Page, expect } from '@playwright/test';

/**
 * Test credentials for authentication
 */
export const TEST_USERS = {
  admin: {
    username: 'admin',
    password: 'Admin@123',
  },
  testuser: {
    username: 'testuser',
    password: 'Test@123',
  },
};

/**
 * API URL for backend
 */
export const API_URL = process.env.REACT_APP_API_URL || 'https://localhost:7057/api';

/**
 * Login helper function
 * @param page - Playwright page object
 * @param username - Username to login with
 * @param password - Password to login with
 */
export async function login(page: Page, username: string, password: string) {
  // Navigate to the login page
  await page.goto('/');
  
  // Wait for login form to be visible
  await page.waitForSelector('input[name="username"], input[type="text"]', { timeout: 10000 });
  
  // Fill in username and password
  const usernameInput = page.locator('input[name="username"], input[type="text"]').first();
  const passwordInput = page.locator('input[name="password"], input[type="password"]').first();
  
  await usernameInput.fill(username);
  await passwordInput.fill(password);
  
  // Click login button
  const loginButton = page.locator('button[type="submit"], button:has-text("Login"), button:has-text("Sign in")').first();
  await loginButton.click();
  
  // Wait for navigation after successful login (typically to dashboard or home page)
  await page.waitForURL((url) => !url.pathname.includes('login'), { timeout: 10000 });
}

/**
 * Login as admin user
 * @param page - Playwright page object
 */
export async function loginAsAdmin(page: Page) {
  await login(page, TEST_USERS.admin.username, TEST_USERS.admin.password);
}

/**
 * Login as test user
 * @param page - Playwright page object
 */
export async function loginAsTestUser(page: Page) {
  await login(page, TEST_USERS.testuser.username, TEST_USERS.testuser.password);
}

/**
 * Logout helper function - attempts multiple strategies to log out
 * @param page - Playwright page object
 */
export async function logout(page: Page) {
  // Strategy 1: Look for visible logout button/link directly
  const logoutButton = page.locator('button:has-text("Logout"), a:has-text("Logout"), button:has-text("Sign out"), a:has-text("Sign out"), button:has-text("Log out"), a:has-text("Log out")').first();
  
  // Check if logout button exists with reasonable timeout
  const logoutExists = await logoutButton.isVisible({ timeout: 5000 }).catch(() => false);
  
  if (logoutExists) {
    // Scroll element into view if needed
    await logoutButton.scrollIntoViewIfNeeded({ timeout: 30000 });
    // Click with extended timeout
    await logoutButton.click({ timeout: 60000, force: false });
    // Wait for redirect to login page
    await page.waitForURL((url) => url.pathname.includes('login') || url.pathname === '/', { timeout: 15000 });
    return;
  }
  
  // Strategy 2: Try to find and click user menu/profile dropdown first
  const userMenu = page.locator('[aria-label="User menu"], [aria-label="Profile"], button:has-text("Profile"), div:has-text("Profile"), .user-menu, .profile-menu').first();
  const userMenuExists = await userMenu.isVisible({ timeout: 5000 }).catch(() => false);
  
  if (userMenuExists) {
    await userMenu.click({ timeout: 10000 });
    // Wait a bit for dropdown to appear
    await page.waitForTimeout(500);
    // Now try logout button again
    const logoutInMenu = page.locator('button:has-text("Logout"), a:has-text("Logout"), button:has-text("Sign out"), a:has-text("Sign out"), button:has-text("Log out"), a:has-text("Log out")').first();
    const logoutInMenuExists = await logoutInMenu.isVisible({ timeout: 5000 }).catch(() => false);
    if (logoutInMenuExists) {
      await logoutInMenu.click({ timeout: 10000 });
      await page.waitForURL((url) => url.pathname.includes('login') || url.pathname === '/', { timeout: 15000 });
      return;
    }
  }
  
  // If we get here, logout button doesn't exist - throw error so test can handle it
  throw new Error('Logout button not found - UI may not have logout functionality');
}

/**
 * Wait for API call to complete
 * @param page - Playwright page object
 * @param urlPattern - URL pattern to wait for
 * @param timeout - Timeout in milliseconds
 */
export async function waitForApiCall(page: Page, urlPattern: string | RegExp, timeout = 10000) {
  await page.waitForResponse(
    (response) => {
      const url = response.url();
      if (typeof urlPattern === 'string') {
        return url.includes(urlPattern);
      }
      return urlPattern.test(url);
    },
    { timeout }
  );
}

/**
 * Take a screenshot with a descriptive name
 * @param page - Playwright page object
 * @param name - Screenshot name
 */
export async function takeScreenshot(page: Page, name: string) {
  await page.screenshot({ 
    path: `screenshots/${name}.png`, 
    fullPage: true 
  });
}

/**
 * Check if element is visible on page
 * @param page - Playwright page object
 * @param selector - CSS selector
 * @returns boolean indicating visibility
 */
export async function isVisible(page: Page, selector: string): Promise<boolean> {
  try {
    const element = page.locator(selector).first();
    return await element.isVisible({ timeout: 5000 });
  } catch {
    return false;
  }
}

/**
 * Check backend health
 * @returns boolean indicating if backend is healthy
 */
export async function checkBackendHealth(): Promise<boolean> {
  try {
    const healthUrl = API_URL.replace('/api', '/health');
    const response = await fetch(healthUrl, {
      method: 'GET',
      headers: {
        'Accept': 'application/json',
      },
    });
    return response.ok;
  } catch (error) {
    console.error('Backend health check failed:', error);
    return false;
  }
}
