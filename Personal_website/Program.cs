using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Personal_website.DB;
using Personal_website.Services;
using Personal_website.Models.Identity;

namespace Personal_website;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        string? identityString = builder.Configuration.GetConnectionString("IdentityConnection");

        if (connectionString == null)
        {
            throw new ArgumentException("The CONNECTION_STRING environment variable is not set");
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
        
        var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!);

        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
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

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        
        app.MapControllers();
        app.Run();
    }
}