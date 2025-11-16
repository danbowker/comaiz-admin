using comaiz.data;
using comaiz.data.Models;
using Microsoft.AspNetCore.Identity;

namespace comaiz.api.Services;

public static class DatabaseSeeder
{
    public static async System.Threading.Tasks.Task SeedDefaultRoles(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        // Seed Roles (always create roles for all deployments)
        string[] roleNames = { "Admin", "User" };
        
        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                await roleManager.CreateAsync(new ApplicationRole { Name = roleName });
                Console.WriteLine($"Created role: {roleName}");
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
