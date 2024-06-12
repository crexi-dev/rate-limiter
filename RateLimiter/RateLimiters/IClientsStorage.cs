using System.Threading.Tasks;

namespace RateLimiter.RateLimiters;

public interface IClientsStorage
{
	Task<(bool, Client)> GetClient(string token);
}
