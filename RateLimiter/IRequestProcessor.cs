using RateLimiter.Rules;
using System.Threading.Tasks;

namespace RateLimiter
{
    /// <summary>
    /// The interface of request processor
    /// </summary>
    public interface IRequestProcessor
    {
        Task<Response> DoRequestAsync(Request request);
    }
}
