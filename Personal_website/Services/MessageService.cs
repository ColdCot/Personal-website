using Microsoft.EntityFrameworkCore;
using Personal_website.DB;
using Personal_website.Models;

namespace Personal_website.Services;

public class MessageService(WebsiteDbContext context) : IMessageService
{
    public async Task<IEnumerable<Message>> GetAllAsync()
    {
        return await context.Messages.ToListAsync();
    }

    public Task<Message?> GetByIdAsync(int id)
    {
        return context.Messages.FirstOrDefaultAsync(m => m.id == id);
    }

    public async Task<IEnumerable<Message>> GetByEmailAsync(string email)
    {
        var senders = context.Senders.Where(s => s.Email == email).Select(s => s.Id);

        if (!await senders.AnyAsync())
        {
            return Enumerable.Empty<Message>();
        }

        return await context.Messages.Where(m => senders.Contains(m.senderId)).ToListAsync();
    }

    public async Task<IEnumerable<Message>> GetByNameAsync(string name)
    {
        var senders = context.Senders.Where(s => s.Name == name).Select(s => s.Id);

        if (!await senders.AnyAsync())
        {
            return Enumerable.Empty<Message>();
        }
        
        return await context.Messages.Where(m => senders.Contains(m.senderId)).ToListAsync();
    }

    public async Task<Message> CreateAsync(string senderName, string senderEmail, string text)
    {
        Sender? sender = await context.Senders.FirstOrDefaultAsync(s => s.Name == senderName && s.Email == senderEmail);
        if (sender == null)
        {
            sender = new Sender
            {
                Name = senderName,
                Email = senderEmail
            };
            
            context.Senders.Add(sender);
            
            await context.SaveChangesAsync();
        }

        Message message = new Message
        {
            senderId = sender.Id,
            text = text,
        };
        
        context.Messages.Add(message);
        await context.SaveChangesAsync();
        
        return message;
    }

    public async Task<Message?> DeleteAsync(int id)
    {
        Message? result = await GetByIdAsync(id);
        if (result == null)
        {
            return result;
        }
        
        context.Messages.Remove(result);
        await context.SaveChangesAsync();
        return result;
    }
}