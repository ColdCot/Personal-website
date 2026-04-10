using Microsoft.EntityFrameworkCore;
using Personal_website.Models;

namespace Personal_website.DB;

public class WebsiteDbContext(DbContextOptions<WebsiteDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Configures the EF Core model for the context, adding a composite unique index on Sender.Name and Sender.Email.
    /// </summary>
    /// <param name="modelBuilder">The ModelBuilder used to configure entity mappings, keys, and indexes.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Sender>()
            .HasIndex(s => new { s.Name, s.Email })
            .IsUnique();
    }
    
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<Sender> Senders => Set<Sender>();
}