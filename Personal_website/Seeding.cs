using Microsoft.AspNetCore.Identity;
using Personal_website.Models.Identity;

namespace Personal_website;

public static class Seeding
{
    public static async Task SeedAdminUser(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
    {
        // Check if any admin user exists
        var existingUser = await userManager.FindByNameAsync("ColdCot");
        if (existingUser != null)
        {
            return; 
        }

        // Get admin password from environment variable or throw an exception
        var adminPassword = configuration["ADMIN_PASSWORD"]
            ?? throw new Exception("Can't find admin password for seeding");

        // Create default admin user
        var adminUser = new User
        {
            UserName = "ColdCot",
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);

        if(!result.Succeeded)
        {
            throw new Exception($"Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }
}