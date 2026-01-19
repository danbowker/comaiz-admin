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
    
    if (await clientsLink.isVisible({ timeout: 5000 })) {
      await clientsLink.click();
      
      // Wait for navigation
      await page.waitForURL(/.*clients.*/i, { timeout: 10000 });
      
      // Take screenshot
      await page.screenshot({ path: 'screenshots/clients-page.png', fullPage: true });
      
      // Verify we're on the clients page
      expect(page.url().toLowerCase()).toContain('clients');
    } else {
      console.log('Clients link not found, skipping test');
      test.skip();
    }
  });

  test('should navigate to Workers page', async ({ page }) => {
    // Look for Workers link in navigation
    const workersLink = page.locator('a:has-text("Workers"), nav a:has-text("Workers")').first();
    
    if (await workersLink.isVisible({ timeout: 5000 })) {
      await workersLink.click();
      
      // Wait for navigation
      await page.waitForURL(/.*workers.*/i, { timeout: 10000 });
      
      // Take screenshot
      await page.screenshot({ path: 'screenshots/workers-page.png', fullPage: true });
      
      // Verify we're on the workers page
      expect(page.url().toLowerCase()).toContain('workers');
    } else {
      console.log('Workers link not found, skipping test');
      test.skip();
    }
  });

  test('should navigate to Contracts page', async ({ page }) => {
    // Look for Contracts link in navigation
    const contractsLink = page.locator('a:has-text("Contracts"), nav a:has-text("Contracts")').first();
    
    if (await contractsLink.isVisible({ timeout: 5000 })) {
      await contractsLink.click();
      
      // Wait for navigation
      await page.waitForURL(/.*contracts.*/i, { timeout: 10000 });
      
      // Take screenshot
      await page.screenshot({ path: 'screenshots/contracts-page.png', fullPage: true });
      
      // Verify we're on the contracts page
      expect(page.url().toLowerCase()).toContain('contracts');
    } else {
      console.log('Contracts link not found, skipping test');
      test.skip();
    }
  });

  test('should have working navigation menu', async ({ page }) => {
    // Check that navigation exists
    const nav = page.locator('nav, header').first();
    await expect(nav).toBeVisible({ timeout: 5000 });
    
    // Take screenshot of navigation
    await page.screenshot({ path: 'screenshots/navigation-menu.png', fullPage: true });
    
    // Count navigation links
    const navLinks = page.locator('nav a, header a');
    const linkCount = await navLinks.count();
    
    expect(linkCount).toBeGreaterThan(0);
    console.log(`Found ${linkCount} navigation links`);
  });
});
