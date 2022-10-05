using Microsoft.EntityFrameworkCore;
using RateLimiter.DAL;

namespace RateLimiter.Tests;

public class Setup
{
    public static ApplicationDbContext GetMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("RateLimiter")
            .Options;
        return new ApplicationDbContext(options);
    }
}