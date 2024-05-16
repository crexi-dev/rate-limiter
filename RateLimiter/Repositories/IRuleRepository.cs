using RateLimiter.Models;
using RateLimiter.Rules;

namespace RateLimiter.Repositories;

public interface IRuleRepository
{
    IRuleCollection GetRules(Token token);
}