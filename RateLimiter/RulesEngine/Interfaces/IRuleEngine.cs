using RateLimiter.Model;

namespace RateLimiter.RulesEngine.Interfaces
{
    public interface IRuleEngine
    {
        bool ProcessRules(ClientRequest ClientRequest);
    }
}