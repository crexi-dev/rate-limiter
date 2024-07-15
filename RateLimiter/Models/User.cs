using RateLimiter.Interfaces.Models;

namespace RateLimiter.Models
{
    public record User(string Id) : IUser;
}
