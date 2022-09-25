using RateLimiter.Models;
using System.Threading.Tasks;

namespace RateLimiter.RuleRunners
{
    public interface IRuleRunner
    {
        Task<RuleRunResult> RunAsync(ClientRequest request);
    }
}
