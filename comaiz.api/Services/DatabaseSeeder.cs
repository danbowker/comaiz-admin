using comaiz.data;
using comaiz.data.Models;
using Microsoft.AspNetCore.Identity;

namespace comaiz.api.Services;

public static class DatabaseSeeder
{
    public static async Task SeedDefaultRolesAndUser(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // Seed Roles
        string[] roleNames = { "Admin", "User" };
        
        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                await roleManager.CreateAsync(new ApplicationRole { Name = roleName });
            }
        }

        // Seed Default Admin User (for testing purposes)
        // In production, this should be done manually or via secure setup
        var adminEmail = "admin@comaiz.local";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        
        if (adminUser == null)
        {
            var newAdminUser = new ApplicationUser
            {
                UserName = "admin",
                Email = adminEmail,
                EmailConfirmed = true
            };
            
            // Default password for development - should be changed immediately
            var result = await userManager.CreateAsync(newAdminUser, "Admin@123");
            
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(newAdminUser, "Admin");
                Console.WriteLine("Default admin user created: admin / Admin@123");
            }
        }

        // Seed Default Test User
        var testEmail = "testuser@comaiz.local";
        var testUser = await userManager.FindByEmailAsync(testEmail);
        
        if (testUser == null)
        {
            var newTestUser = new ApplicationUser
            {
                UserName = "testuser",
                Email = testEmail,
                EmailConfirmed = true
            };
            
            var result = await userManager.CreateAsync(newTestUser, "Test@123");
            
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(newTestUser, "User");
                Console.WriteLine("Default test user created: testuser / Test@123");
            }
        }
    }
}
