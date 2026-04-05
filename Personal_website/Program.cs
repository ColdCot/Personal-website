using Microsoft.EntityFrameworkCore;
using Personal_website.DB;
using Personal_website.Services;

namespace Personal_website;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        if (connectionString == null)
        {
            throw new ArgumentException("The CONNECTION_STRING environment variable is not set");
        }

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddDbContext<WebsiteDbContext>(options => 
            options.UseSqlServer(connectionString));
        
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