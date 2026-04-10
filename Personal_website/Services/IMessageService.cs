using Personal_website.Models;

namespace Personal_website.Services;

public interface IMessageService
{
    public Task<IEnumerable<Message>> GetAllAsync();
    public Task<Message?> GetByIdAsync(int id);
    public Task<IEnumerable<Message>> GetByEmailAsync(string email);
    public Task<IEnumerable<Message>> GetByNameAsync(string name);
    public Task<Message> CreateAsync(string senderName, string senderEmail, string text);
    public Task<Message?> DeleteAsync(int id);
}