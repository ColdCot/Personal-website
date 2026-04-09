using Microsoft.AspNetCore.Identity;
using Personal_website.Models.Identity;

namespace Personal_website;

public class Seeding
{
    public static async Task SeedAdminUser(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
    {
        // Check if any admin user exists
        var adminUsers = await userManager.GetUsersInRoleAsync("Admin");
        if (adminUsers.Any())
        {
            return; // Admin user already exists
        }

        // Create Admin role if it doesn't exist
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        // Get admin password from environment variable or use default
        var adminPassword = configuration["ADMIN_PASSWORD"] ?? "Admin@12345";

        // Create default admin user
        var adminUser = new User
        {
            UserName = "admin",
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);

        if (result.Succeeded)
        {
            // Assign Admin role to the user
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
        else
        {
            throw new Exception($"Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }
}