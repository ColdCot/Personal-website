using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Personal_website.Models;
using Personal_website.Services;

namespace Personal_website.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SendersController(ISenderService senderService) : ControllerBase
{
    /// <summary>
    /// Retrieves all Sender entities
    /// </summary>
    /// <returns>An IEnumerable of all Sender objects</returns>
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Sender>>> GetAllAsync()
    {
        return Ok(await senderService.GetAllAsync());
    }
}