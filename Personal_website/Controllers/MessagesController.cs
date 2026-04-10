using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Personal_website.DTO;
using Personal_website.Models;
using Personal_website.Services;

namespace Personal_website.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessagesController(IMessageService messageService) : ControllerBase
{
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Message>>> GetAllAsync()
    {
        return Ok(await messageService.GetAllAsync());
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<Message>> GetByIdAsync([FromRoute]int id)
    {
        var result = await messageService.GetByIdAsync(id);
        return result != null ? Ok(result) : NotFound();
    }

    [Authorize]
    [HttpGet("email")]
    public async Task<ActionResult<IEnumerable<Message>>> GetByEmailAsync([FromQuery]string email)
    {
        return Ok(await messageService.GetByEmailAsync(email));
    }
    
    [Authorize]
    [HttpGet("name")]
    public async Task<ActionResult<IEnumerable<Message>>> GetByNameAsync([FromQuery]string name)
    {
        return Ok(await messageService.GetByNameAsync(name));
    }

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

            return Ok(newMessage);
        }
        catch (DbUpdateException)
        {
            return Conflict("Failed to create message");
        }
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync([FromRoute]int id)
    {
        var result = await messageService.DeleteAsync(id);
        if (result == null)
        {
            return NotFound();
        }
        
        return Ok();
    }
}