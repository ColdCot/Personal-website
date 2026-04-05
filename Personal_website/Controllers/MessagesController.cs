using Microsoft.AspNetCore.Mvc;
using Personal_website.Models;
using Personal_website.Services;

namespace Personal_website.Controllers;

[ApiController]
[Route("[controller]")]
public class MessagesController(IMessageService messageService) : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<Message>> GetAll()
    {
        return Ok(messageService.GetAll());
    }

    [HttpGet("{id}")]
    public ActionResult<Message> GetById(int id)
    {
        return Ok(messageService.GetById(id));
    }

    [HttpGet("email")]
    public ActionResult<IEnumerable<Message>> GetByEmail(string email)
    {
        return Ok(messageService.GetByEmail(email));
    }
    
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

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        int result = messageService.Delete(id);

        if (result == 1)
        {
            return BadRequest();
        }
        
        return Ok();
    }
}