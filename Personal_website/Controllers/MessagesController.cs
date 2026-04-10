using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Personal_website.DTO;
using Personal_website.Models;
using Personal_website.Services;

namespace Personal_website.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessagesController(IMessageService messageService, ILogger<MessagesController> logger) : ControllerBase
{
    /// <summary>
    /// Retrieves all message entities.
    /// </summary>
    /// <returns>An IEnumerable of all Message objects.</returns>
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Message>>> GetAllAsync()
    {
        return Ok(await messageService.GetAllAsync());
    }

    /// <summary>
    /// Retrieves the message with the specified identifier.
    /// </summary>
    /// <param name="id">The identifier of the message to retrieve.</param>
    /// <returns>The requested <see cref="Message"/> when found; otherwise a 404 NotFound result.</returns>
    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<Message>> GetByIdAsync([FromRoute]int id)
    {
        var result = await messageService.GetByIdAsync(id);
        return result != null ? Ok(result) : NotFound();
    }

    /// <summary>
    /// Retrieves messages that match the specified sender email.
    /// </summary>
    /// <param name="email">The sender email used to filter messages.</param>
    /// <returns>A collection of <see cref="Message"/> objects matching the provided email.</returns>
    [Authorize]
    [HttpGet("email")]
    public async Task<ActionResult<IEnumerable<Message>>> GetByEmailAsync([FromQuery]string email)
    {
        return Ok(await messageService.GetByEmailAsync(email));
    }
    
    /// <summary>
    /// Retrieves all messages that match the specified name.
    /// </summary>
    /// <param name="name">The name to filter messages by.</param>
    /// <returns>An <see cref="IEnumerable{Message}"/> containing messages that match the provided name.</returns>
    [Authorize]
    [HttpGet("name")]
    public async Task<ActionResult<IEnumerable<Message>>> GetByNameAsync([FromQuery]string name)
    {
        return Ok(await messageService.GetByNameAsync(name));
    }

    /// <summary>
    /// Creates a new Message from the provided request and returns the created resource.
    /// </summary>
    /// <param name="requestDto">Request data containing SenderName, SenderEmail, and Text for the new message.</param>
    /// <returns>`201 Created` with the created Message when successful; `500 Internal Server Error` with the body "Internal server error" if creation fails.</returns>
    [HttpPost]
    public async Task<ActionResult<Message>> CreateAsync([FromBody] MessageRequestDto requestDto)
    {
        try
        {
            var newMessage = await messageService.CreateAsync(
                requestDto.SenderName,
                requestDto.SenderEmail,
                requestDto.Text
            );

            return CreatedAtAction(nameof(GetByIdAsync), new { id = newMessage.id }, newMessage);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create message");
            
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }

    /// <summary>
    /// Deletes the message with the specified identifier and deletes the sender if they have no messages left.
    /// </summary>
    /// <param name="id">The identifier of the message to delete.</param>
    /// <returns>`200 OK` if the message was deleted; `404 NotFound` if no message with the specified id exists; `500 Internal Server Error` if message deletion fails.</returns>
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync([FromRoute]int id)
    {
        Message? result;
        try
        {
            result = await messageService.DeleteAsync(id);
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "Failed to delete message");
            
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }

        if (result == null)
        {
            return NotFound();
        }
        
        return Ok();
    }
}