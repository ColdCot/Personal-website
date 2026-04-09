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
        //TODO: Uncomment before deployment
        /*using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var userManager = services.GetRequiredService<UserManager<User>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            SeedAdminUser(userManager, roleManager, builder.Configuration).Wait();
        }*/

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
}