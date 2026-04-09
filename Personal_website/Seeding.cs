using Microsoft.AspNetCore.Identity;
using Personal_website.Models.Identity;

namespace Personal_website;

public class Seeding
{
    public static async Task SeedAdminUser(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
    {
        // Check if any admin user exists
        var Users = userManager.Users;
        if (Users.Any())
        {
            return; 
        }

        // Get admin password from environment variable or throw an exception
        var adminPassword = configuration["ADMIN_PASSWORD"]
            ?? throw new Exception("Cant find admin password for seeding");

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