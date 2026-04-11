using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Personal_website.DB;
using Personal_website.Models;
using Personal_website.Services;

namespace XUnitTest;

public class SenderServiceTest
{
    [Fact]
    public async Task ReturnsEmptyIfTableIsEmpty()
    {
        var options = new DbContextOptionsBuilder<WebsiteDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new WebsiteDbContext(options);
        var service = new SenderService(context);
        var result = await service.GetAllAsync();
            
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ReturnsSendersWhenNotEmpty()
    {
        var options = new DbContextOptionsBuilder<WebsiteDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var data = new List<Sender>
        {
            new Sender { Id = 1, Name = "Test1", Email = "test1@gmail.com"},
            new Sender { Id = 2, Name = "Test2", Email = "test2@gmail.com" },
            new Sender { Id = 3, Name = "Test3", Email = "test3@gmail.com" },
            new Sender { Id = 4, Name = "Test4", Email = "test4@gmail.com" },
            new Sender { Id = 5, Name = "Test5", Email = "test5@gmail.com" },
            new Sender { Id = 6, Name = "Test6", Email = "test6@gmail.com" },
        };

        await using var context = new WebsiteDbContext(options);
        var service = new SenderService(context);

        foreach (var item in data)
        {
            context.Senders.Add(item);
        }
        
        await context.SaveChangesAsync();
        
        var result = (await service.GetAllAsync()).ToList();
        
        result.Should().BeEquivalentTo(data);
    }
}