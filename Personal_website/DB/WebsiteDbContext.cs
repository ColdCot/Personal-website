using Microsoft.EntityFrameworkCore;
using Personal_website.Models;

namespace Personal_website.DB;

public class WebsiteDbContext(DbContextOptions<WebsiteDbContext> options) : DbContext(options)
{
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<Sender> Senders => Set<Sender>();
}