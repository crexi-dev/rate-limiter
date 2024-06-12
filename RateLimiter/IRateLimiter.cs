using System.Threading.Tasks;

namespace RateLimiter;

public interface IRateLimiter
{
	Task<bool> AllowRequest(string resource, string token);
}
