using RateLimiter.Contexts.Interfaces;

namespace RateLimiter.Contexts;

public class UserContext : IUserContext
{
    public string Region { get; set; } = null!;
}