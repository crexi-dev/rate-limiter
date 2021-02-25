using System.Threading.Tasks;

namespace RateLimiter
{
    public interface IEndpointService
    {
        Task<string> GetEndPoint(string accessToken);
    }
}
