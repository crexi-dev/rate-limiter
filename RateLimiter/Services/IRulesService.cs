using RateLimiter.Services.Rules;
using RateLimiter.Services.Rules.Models;

namespace RateLimiter.Services;

public interface IRulesService
{
    public IRule GetRule(ClientInfo clientInfo);
}