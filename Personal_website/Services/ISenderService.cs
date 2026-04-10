using Personal_website.Models;

namespace Personal_website.Services;

public interface ISenderService
{
    /// <summary>
    /// Retrieves all stored senders
    /// </summary>
    /// <returns>An enumerable containing every <see cref="Sender"/>></returns>
    public Task<IEnumerable<Sender>> GetAllAsync();
}