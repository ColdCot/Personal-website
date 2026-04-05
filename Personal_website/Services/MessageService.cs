using Personal_website.DB;
using Personal_website.Models;

namespace Personal_website.Services;

public class MessageService(WebsiteDbContext context) : IMessageService
{
    public IEnumerable<Message> GetAll()
    {
        return context.Messages;
    }

    public Message? GetById(int id)
    {
        return context.Messages.FirstOrDefault(m => m.id == id);
    }

    public IEnumerable<Message> GetByEmail(string email)
    {
        var senders = context.Senders.Where(s => s.Email == email).Select(s => s.Id);

        if (!senders.Any())
        {
            return Enumerable.Empty<Message>();
        }
        
        IEnumerable<Message> messages = context.Messages.Where(m => senders.Contains(m.senderId));
        return messages;
    }

    public IEnumerable<Message> GetByName(string name)
    {
        var senders = context.Senders.Where(s => s.Name == name).Select(s => s.Id);

        if (!senders.Any())
        {
            return Enumerable.Empty<Message>();
        }
        
        IEnumerable<Message> messages = context.Messages.Where(m => senders.Contains(m.senderId));
        return messages;
    }

    public Message Create(string senderName, string senderEmail, string text)
    {
        Sender? sender = context.Senders.FirstOrDefault(s => s.Name == senderName && s.Email == senderEmail);
        if (sender == null)
        {
            sender = new Sender
            {
                Name = senderName,
                Email = senderEmail
            };
            
            context.Senders.Add(sender);
            
            context.SaveChanges();
        }

        Message message = new Message
        {
            senderId = sender.Id,
            text = text,
        };
        
        context.Messages.Add(message);
        context.SaveChanges();
        
        return message;
    }

    public Message? Delete(int id)
    {
        Message? result = GetById(id);
        if (result == null)
        {
            return result;
        }
        
        context.Messages.Remove(result);
        context.SaveChanges();
        return result;
    }
}