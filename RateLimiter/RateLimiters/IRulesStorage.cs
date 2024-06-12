using System.Threading.Tasks;

namespace RateLimiter.RateLimiters;

public interface IRulesStorage
{
	Task<IRule> GetRule(string resource);
}
