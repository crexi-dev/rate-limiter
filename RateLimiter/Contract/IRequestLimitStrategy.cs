using System.Threading.Tasks;

namespace RateLimiter.Contract
{
    internal interface IRequestLimitStrategy
    {
        /// <summary>
        /// Verifies if the request can go through
        /// </summary>
        /// <param name="request">A request (containing client identifier). For simplicity it is just a field in Request class (that is taken from access token)/param>
        /// <returns></returns>
        Task<bool> CanPassThroughAsync(Request request);
    }
}
