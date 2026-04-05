using Personal_website.Models;

namespace Personal_website.Services;

public interface IMessageService
{
    public IEnumerable<Message> GetAll();
    public Message? GetById(int id);
    public IEnumerable<Message> GetByEmail(string email);
    public IEnumerable<Message> GetByName(string name);
    public Message Create(string senderName, string senderEmail, string text);
    public int Delete(int id);
}