using Microsoft.EntityFrameworkCore;
using Personal_website.Models;

namespace Personal_website.DB;

public class WebsiteDbContext(DbContextOptions<WebsiteDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Sender>()
            .HasIndex(s => new { s.Name, s.Email })
            .IsUnique();
    }
    
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<Sender> Senders => Set<Sender>();
}