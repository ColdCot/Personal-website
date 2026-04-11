using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Personal_website.DB;
using Personal_website.Models;
using Personal_website.Services;

namespace XUnitTest;

public class SenderServiceTest
{ 
    private static (WebsiteDbContext context, SenderService service) CreateTestEnvironment()
    {
        var options = new DbContextOptionsBuilder<WebsiteDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    
        var context = new WebsiteDbContext(options);
        var service = new SenderService(context);
    
        return (context, service);
    }
    [Fact]
    public async Task ReturnsEmptyIfTableIsEmpty()
    {
        var (context, service) = CreateTestEnvironment();
        
        await using (context)
        {
            var result = await service.GetAllAsync();
            result.Should().BeEmpty();
        }
    }

    [Fact]
    public async Task ReturnsSendersWhenNotEmpty()
    {
        var (context, service) = CreateTestEnvironment();

        await using (context)
        {
            var data = new List<Sender>
            {
                new Sender { Id = 1, Name = "Test1", Email = "test1@gmail.com" },
                new Sender { Id = 2, Name = "Test2", Email = "test2@gmail.com" },
                new Sender { Id = 3, Name = "Test3", Email = "test3@gmail.com" },
                new Sender { Id = 4, Name = "Test4", Email = "test4@gmail.com" },
                new Sender { Id = 5, Name = "Test5", Email = "test5@gmail.com" },
                new Sender { Id = 6, Name = "Test6", Email = "test6@gmail.com" },
            };

            context.Senders.AddRange(data);

            await context.SaveChangesAsync();

            var result = await service.GetAllAsync();

            result.Should().BeEquivalentTo(data);
        }
    }
}