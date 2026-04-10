using Personal_website.Models;

namespace Personal_website.Services;

public interface IMessageService
{
    /// <summary>
/// Retrieves all stored messages.
/// </summary>
/// <returns>An enumerable containing every <see cref="Message"/> currently stored.</returns>
public Task<IEnumerable<Message>> GetAllAsync(); 
    /// <summary>
/// Retrieves a message by its numeric identifier.
/// </summary>
/// <param name="id">The numeric identifier of the message to retrieve.</param>
/// <returns>The <see cref="Message"/> with the specified id, or <c>null</c> if no such message exists.</returns>
public Task<Message?> GetByIdAsync(int id);
    /// <summary>
/// Retrieves all messages that match the specified sender email.
/// </summary>
/// <param name="email">The sender email to match.</param>
/// <returns>A collection of messages with the specified sender email.</returns>
public Task<IEnumerable<Message>> GetByEmailAsync(string email);
    /// <summary>
/// Retrieves messages sent by the specified sender name.
/// </summary>
/// <param name="name">Sender name to match.</param>
/// <returns>An <see cref="IEnumerable{Message}"/> containing messages with the specified sender name; empty if none are found.</returns>
public Task<IEnumerable<Message>> GetByNameAsync(string name);
    /// <summary>
/// Creates a new Message using the provided sender name, sender email, and message text.
/// </summary>
/// <param name="senderName">The display name of the message sender.</param>
/// <param name="senderEmail">The email address of the message sender.</param>
/// <param name="text">The body text of the message.</param>
/// <returns>The created <see cref="Message"/>.</returns>
public Task<Message> CreateAsync(string senderName, string senderEmail, string text);
    /// <summary>
/// Deletes the message with the specified identifier.
/// </summary>
/// <param name="id">The numeric identifier of the message to delete.</param>
/// <returns>The deleted <see cref="Message"/> if it existed; otherwise <c>null</c>.</returns>
public Task<Message?> DeleteAsync(int id);
}