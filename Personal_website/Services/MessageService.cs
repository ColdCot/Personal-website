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

    public async Task<Message?> GetByIdAsync(int id)
    {
        return await context.Messages.FirstOrDefaultAsync(m => m.id == id);
    }

    public async Task<IEnumerable<Message>> GetByEmailAsync(string email)
    {
        var senders = context.Senders.Where(s => s.Email == email).Select(s => s.Id);

        return await context.Messages.Where(m => senders.Contains(m.senderId)).ToListAsync();
    }

    public async Task<IEnumerable<Message>> GetByNameAsync(string name)
    {
        var senders = context.Senders.Where(s => s.Name == name).Select(s => s.Id);
        
        return await context.Messages.Where(m => senders.Contains(m.senderId)).ToListAsync();
    }

    public async Task<Message> CreateAsync(string senderName, string senderEmail, string text)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();
        
        try
        {
            Sender? sender =
                await context.Senders.FirstOrDefaultAsync(s => s.Name == senderName && s.Email == senderEmail);
            if (sender == null)
            {
                sender = new Sender
                {
                    Name = senderName,
                    Email = senderEmail
                };

                context.Senders.Add(sender);

                try
                {
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    context.Entry(sender).State = EntityState.Detached;
                    
                    sender = await context.Senders.AsNoTracking()
                        .FirstOrDefaultAsync(s => s.Name == senderName && s.Email == senderEmail);
                
                    if (sender == null) throw;
                }
            }

            Message message = new Message
            {
                senderId = sender.Id,
                text = text,
            };

            context.Messages.Add(message);
            await context.SaveChangesAsync();
            
            await transaction.CommitAsync();

            return message;
        }
        catch(DbUpdateException)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<Message?> DeleteAsync(int id)
    {
        Message? result = await GetByIdAsync(id);
        if (result == null) return null;
            
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            context.Messages.Remove(result);
            await context.SaveChangesAsync();

            //removing senders with no messages
            await context.Senders
                .Where(s => s.Id == result.senderId &&
                            !context.Messages.Any(m => m.senderId == s.Id))
                .ExecuteDeleteAsync();

            await transaction.CommitAsync();
        }
        catch (DbUpdateException)
        {
            await transaction.RollbackAsync();
            throw;
        }

        return result;
    }
}