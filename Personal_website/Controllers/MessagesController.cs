using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public ActionResult<IEnumerable<Message>> GetAll()
    {
        return Ok(messageService.GetAll());
    }

    [Authorize]
    [HttpGet("{id}")]
    public ActionResult<Message> GetById(int id)
    {
        return Ok(messageService.GetById(id));
    }

    [Authorize]
    [HttpGet("email")]
    public ActionResult<IEnumerable<Message>> GetByEmail(string email)
    {
        return Ok(messageService.GetByEmail(email));
    }
    
    [Authorize]
    [HttpGet("name")]
    public ActionResult<IEnumerable<Message>> GetByName(string name)
    {
        return Ok(messageService.GetByName(name));
    }

    [HttpPost]
    public ActionResult<Message> Create([FromBody] MessageRequest request)
    {
        var newMessage = messageService.Create(
            request.SenderName, 
            request.SenderEmail, 
            request.Text
        );

        return Ok(newMessage);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var result = messageService.Delete(id);
        if (result == null)
        {
            return NotFound();
        }
        
        return Ok();
    }
}