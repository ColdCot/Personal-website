using Microsoft.EntityFrameworkCore;
using Personal_website.DB;
using Personal_website.Models;

namespace Personal_website.Services;

public class SenderService(WebsiteDbContext context) : ISenderService
{
    public async Task<IEnumerable<Sender>> GetAllAsync()
    {
        return await context.Senders.AsNoTracking().ToListAsync();
    }
}