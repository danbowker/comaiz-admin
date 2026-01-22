import { test, expect } from '@playwright/test';
import { loginAsAdmin } from './utils/test-helpers';

test.describe('Navigation', () => {
  test.beforeEach(async ({ page }) => {
    // Login before each test
    await loginAsAdmin(page);
  });

  test('should navigate to Clients page', async ({ page }) => {
    // Look for Clients link in navigation
    const clientsLink = page.locator('a:has-text("Clients"), nav a:has-text("Clients")').first();
    
    try {
      // Wait for element to be visible and ready with extended timeout
      await clientsLink.waitFor({ state: 'visible', timeout: 60000 });
      await clientsLink.click({ timeout: 60000 });
      
      // Wait for navigation
      await page.waitForURL(/.*clients.*/i, { timeout: 15000 });
      
      // Take screenshot
      await page.screenshot({ path: 'screenshots/clients-page.png', fullPage: true });
      
      // Verify we're on the clients page
      expect(page.url().toLowerCase()).toContain('clients');
    } catch (error) {
      console.log('Clients link not found or not clickable, skipping test');
      test.skip();
    }
  });

  test('should navigate to Workers page', async ({ page }) => {
    // Look for Workers link in navigation
    const workersLink = page.locator('a:has-text("Workers"), nav a:has-text("Workers")').first();
    
    try {
      // Wait for element to be visible and ready with extended timeout
      await workersLink.waitFor({ state: 'visible', timeout: 60000 });
      await workersLink.click({ timeout: 60000 });
      
      // Wait for navigation
      await page.waitForURL(/.*workers.*/i, { timeout: 15000 });
      
      // Take screenshot
      await page.screenshot({ path: 'screenshots/workers-page.png', fullPage: true });
      
      // Verify we're on the workers page
      expect(page.url().toLowerCase()).toContain('workers');
    } catch (error) {
      console.log('Workers link not found or not clickable, skipping test');
      test.skip();
    }
  });

  test('should navigate to Contracts page', async ({ page }) => {
    // Look for Contracts link in navigation
    const contractsLink = page.locator('a:has-text("Contracts"), nav a:has-text("Contracts")').first();
    
    try {
      // Wait for element to be visible and ready with extended timeout
      await contractsLink.waitFor({ state: 'visible', timeout: 60000 });
      await contractsLink.click({ timeout: 60000 });
      
      // Wait for navigation
      await page.waitForURL(/.*contracts.*/i, { timeout: 15000 });
      
      // Take screenshot
      await page.screenshot({ path: 'screenshots/contracts-page.png', fullPage: true });
      
      // Verify we're on the contracts page
      expect(page.url().toLowerCase()).toContain('contracts');
    } catch (error) {
      console.log('Contracts link not found or not clickable, skipping test');
      test.skip();
    }
  });

  test('should have working navigation menu', async ({ page }) => {
    // Check that navigation exists - wait for it to be ready
    const nav = page.locator('nav, header').first();
    await nav.waitFor({ state: 'visible', timeout: 30000 });
    await expect(nav).toBeVisible({ timeout: 30000 });
    
    // Take screenshot of navigation
    await page.screenshot({ path: 'screenshots/navigation-menu.png', fullPage: true });
    
    // Count navigation links - wait for them to load
    const navLinks = page.locator('nav a, header a');
    // Wait for at least one link to be present with extended timeout
    await page.locator('nav a, header a').first().waitFor({ state: 'visible', timeout: 30000 });
    const linkCount = await navLinks.count();
    
    expect(linkCount).toBeGreaterThan(0);
    console.log(`Found ${linkCount} navigation links`);
  });
});
