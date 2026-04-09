using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Personal_website.DB;
using Personal_website.Services;
using Personal_website.Models.Identity;
using Personal_website.Options;

namespace Personal_website;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.Configure<JwtOptions>(
            builder.Configuration.GetSection("Jwt"));
        
        string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        string? identityString = builder.Configuration.GetConnectionString("IdentityConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException("The CONNECTION_STRING environment variable is not set");
        }

        if (string.IsNullOrWhiteSpace(identityString))
        {
            throw new ArgumentException("The IDENTITY STRING environment variable is not set");
        }

        // Add services to the container.
        builder.Services.AddControllers();
        
        builder.Services.AddDbContext<AuthDbContext>(options => 
            options.UseSqlServer(identityString));
        
        builder.Services.AddDbContext<WebsiteDbContext>(options => 
            options.UseSqlServer(connectionString));

        builder.Services.AddIdentity<User, IdentityRole>()
            .AddEntityFrameworkStores<AuthDbContext>()
            .AddDefaultTokenProviders();
        
        var jwtOptions = builder.Configuration
            .GetSection("Jwt")
            .Get<JwtOptions>()
            ??throw new ArgumentException("The JWT environment variable is not set");

        if (string.IsNullOrWhiteSpace(jwtOptions.Key))
        {
            throw new ArgumentException("The JWT key environment variable is not set");
        }

        if (string.IsNullOrWhiteSpace(jwtOptions.Issuer))
        {
            throw new ArgumentException("The JWT issuer environment variable is not set");
        }

        if (string.IsNullOrWhiteSpace(jwtOptions.Audience))
        {
            throw new ArgumentException("The JWT Audience environment variable is not set");
        }
        
        var key = Encoding.UTF8.GetBytes(jwtOptions.Key);

        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                };
            });
        
        builder.Services.AddAuthorization();
        
        builder.Services.AddScoped<IMessageService, MessageService>();
        
        builder.Services.AddOpenApi();
        builder.Services.AddEndpointsApiExplorer(); 
        builder.Services.AddSwaggerGen(); 

        var app = builder.Build();

        // Seed initial admin user
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var userManager = services.GetRequiredService<UserManager<User>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            SeedAdminUser(userManager, roleManager, builder.Configuration).Wait();
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();

            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.Run();
    }

    private static async Task SeedAdminUser(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
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
            Email = "admin@example.com",
            EmailConfirmed = true
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