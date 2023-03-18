using RateLimiter.Services.Rules.Models;

namespace RateLimiter.Services.Rules;

public interface IRule
{
    public bool IsAllowed(ClientInfo clientInfo);
}