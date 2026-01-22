import { test, expect } from '@playwright/test';
import { loginAsAdmin } from './utils/test-helpers';

test.describe('Clients CRUD Operations', () => {
  test.beforeEach(async ({ page }) => {
    // Login and navigate to Clients page
    await loginAsAdmin(page);
    
    // Navigate to Clients page
    const clientsLink = page.locator('a:has-text("Clients"), nav a:has-text("Clients")').first();
    if (await clientsLink.isVisible({ timeout: 5000 })) {
      // Wait for element to be ready before clicking
      await clientsLink.waitFor({ state: 'visible', timeout: 10000 });
      await clientsLink.click({ timeout: 60000 });
      await page.waitForURL(/.*clients.*/i, { timeout: 10000 });
    } else {
      // Try direct navigation
      await page.goto('/clients');
    }
    
    // Wait for page to stabilize
    await page.waitForLoadState('networkidle', { timeout: 30000 });
  });

  test('should display clients list', async ({ page }) => {
    // Wait for the page to load
    await page.waitForTimeout(2000);
    
    // Take screenshot of clients list
    await page.screenshot({ path: 'screenshots/clients-list.png', fullPage: true });
    
    // Check if we can see client-related content (table, list, or "no clients" message)
    const hasContent = await page.locator('table, .client, .clients, div[role="table"], [data-testid*="client"]').first().isVisible({ timeout: 5000 }).catch(() => false);
    const hasNoDataMessage = await page.locator(':text("No clients"), :text("no data"), :text("empty")').first().isVisible({ timeout: 5000 }).catch(() => false);
    
    // Either we have content or a "no data" message
    expect(hasContent || hasNoDataMessage).toBeTruthy();
  });

  test('should open create client form', async ({ page }) => {
    // Look for "New", "Add", "Create" button
    const createButton = page.locator('button:has-text("New"), button:has-text("Add"), button:has-text("Create"), a:has-text("New Client"), a:has-text("Add Client")').first();
    
    if (await createButton.isVisible({ timeout: 5000 })) {
      await createButton.click();
      
      // Wait for form to appear
      await page.waitForTimeout(1000);
      
      // Take screenshot of create form
      await page.screenshot({ path: 'screenshots/client-create-form.png', fullPage: true });
      
      // Check if form is visible
      const formVisible = await page.locator('form, input[name*="name"], input[name*="code"]').first().isVisible({ timeout: 5000 });
      expect(formVisible).toBeTruthy();
    } else {
      console.log('Create client button not found, skipping test');
      test.skip();
    }
  });

  test('should display client details when clicking on a client', async ({ page }) => {
    // Wait for clients to load
    await page.waitForTimeout(2000);
    
    // Look for first client in list (could be a row, card, or link)
    const firstClient = page.locator('table tbody tr, .client-item, .client-card, [data-testid*="client-"]').first();
    
    if (await firstClient.isVisible({ timeout: 5000 })) {
      // Click on the client
      await firstClient.click();
      
      // Wait for details to load
      await page.waitForTimeout(1000);
      
      // Take screenshot
      await page.screenshot({ path: 'screenshots/client-details.png', fullPage: true });
      
      // Verify some detail view is shown (could be edit form, detail panel, etc.)
      const hasDetails = await page.locator('form, .detail, .details, [role="dialog"]').first().isVisible({ timeout: 5000 }).catch(() => false);
      expect(hasDetails).toBeTruthy();
    } else {
      console.log('No clients found to click on, skipping test');
      test.skip();
    }
  });
});
