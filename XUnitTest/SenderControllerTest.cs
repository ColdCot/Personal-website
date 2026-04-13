using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Personal_website.Controllers;
using Personal_website.Models;
using Personal_website.Services;

namespace XUnitTest;

public class SenderControllerTest
{
    [Fact]
    public async Task ReturnsOkIfSuccess()
    {
        List<Sender> data = new List<Sender>()
        {
            new Sender()
            {
                Id = 1,
                Email = "test@test.com",
                Name = "Test",
            },
            new Sender()
            {
                Id = 2,
                Email = "test2@test.com",
                Name = "Test2",
            }
        };
        
        SendersController sendersController = new SendersController(new SenderServiceMock(data));

        var actionResult = await sendersController.GetAllAsync();
        var result = actionResult.Result as OkObjectResult;

        actionResult.Should().BeOfType<ActionResult<IEnumerable<Sender>>>();
        result.Should().NotBeNull();
        var content = result.Value as IEnumerable<Sender>;
        content.Should().BeEquivalentTo(data);
    }

    [Fact]
    public async Task ReturnsOkAndEmptyIfListIsEmpty()
    {
        SendersController sendersController = new SendersController(new SenderServiceMock(new List<Sender>()));
        
        var actionResult = await sendersController.GetAllAsync();
        var result = actionResult.Result as OkObjectResult;
        
        actionResult.Should().BeOfType<ActionResult<IEnumerable<Sender>>>();
        result.Should().NotBeNull();
        var content = result.Value as IEnumerable<Sender>;
        content.Should().BeEmpty();
    }
}

class SenderServiceMock : ISenderService
{
    private List<Sender> Data { get; }

    public SenderServiceMock(List<Sender> data)
    {
        this.Data = data;
    }
    
    public async Task<IEnumerable<Sender>> GetAllAsync()
    {
        return await Task.FromResult(this.Data);
    }
}