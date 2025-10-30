using comaiz.data;
using comaiz.data.Models;
using Microsoft.AspNetCore.Identity;

namespace comaiz.api.Services;

public static class DatabaseSeeder
{
    public static async System.Threading.Tasks.Task SeedDefaultRolesAndUser(IServiceProvider serviceProvider, bool isDevelopment = false)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // Seed Roles (always create roles)
        string[] roleNames = { "Admin", "User" };
        
        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                await roleManager.CreateAsync(new ApplicationRole { Name = roleName });
            }
        }

        // Only seed default users in Development environment
        if (!isDevelopment)
        {
            Console.WriteLine("Skipping default user creation in non-Development environment.");
            Console.WriteLine("Use Add-ComaizUser.ps1 script or DatabaseSeeder to create users.");
            return;
        }

        Console.WriteLine("Development environment detected - creating default users...");

        // Seed Default Admin User (for development/testing purposes only)
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
                Console.WriteLine("WARNING: Change this password immediately in production!");
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

    public static async Task<bool> CreateUser(IServiceProvider serviceProvider, string username, string email, string password, string role)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        try
        {
            // Ensure role exists
            if (!await roleManager.RoleExistsAsync(role))
            {
                var roleResult = await roleManager.CreateAsync(new ApplicationRole { Name = role });
                if (!roleResult.Succeeded)
                {
                    Console.WriteLine($"Error creating role: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                    return false;
                }
            }

            // Check if user already exists
            var existingUser = await userManager.FindByNameAsync(username);
            if (existingUser != null)
            {
                Console.WriteLine($"Error: User '{username}' already exists");
                return false;
            }

            existingUser = await userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                Console.WriteLine($"Error: Email '{email}' is already in use");
                return false;
            }

            // Create user
            var user = new ApplicationUser
            {
                UserName = username,
                Email = email,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                Console.WriteLine($"Error creating user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                return false;
            }

            // Assign role
            var roleAssignResult = await userManager.AddToRoleAsync(user, role);
            if (!roleAssignResult.Succeeded)
            {
                Console.WriteLine($"Error assigning role: {string.Join(", ", roleAssignResult.Errors.Select(e => e.Description))}");
                return false;
            }

            Console.WriteLine($"Success: User '{username}' created with role '{role}'");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return false;
        }
    }
}
