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
    /// <summary>
    /// Configures and runs the ASP.NET Core web application: registers services (controllers, DbContexts, Identity, authentication/authorization, scoped application services, and OpenAPI), validates required configuration (connection strings and JWT settings), seeds an initial admin user, configures the HTTP request pipeline, and starts the server.
    /// </summary>
    /// <param name="args">Command-line arguments passed to the application.</param>
    /// <exception cref="ArgumentException">Thrown when required configuration is missing: DefaultConnection, IdentityConnection, or any required JwtOptions field (Key, Issuer, Audience).</exception>
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
        builder.Services.AddControllers(options =>
        {
            options.SuppressAsyncSuffixInActionNames = false;
        });
        
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
        builder.Services.AddScoped<ISenderService, SenderService>();
        
        builder.Services.AddOpenApi();
        builder.Services.AddEndpointsApiExplorer(); 
        builder.Services.AddSwaggerGen(); 

        var app = builder.Build();
        
        //seed first user
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var userManager = services.GetRequiredService<UserManager<User>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            Seeding.SeedAdminUser(userManager, roleManager, builder.Configuration).GetAwaiter().GetResult();
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
}