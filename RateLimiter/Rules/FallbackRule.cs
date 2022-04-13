using RateLimiter.Models;
using System.Threading;
using System.Threading.Tasks;

namespace RateLimiter.Rules
{
    /// <summary>
    /// A fallback rule. Deny any requests that are not otherwise handled by another rule
    /// </summary>
    public class FallbackRule : RateLimiterBaseRule
    {
        protected override Task<bool> IsApplicableAsync(RequestInfo request, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        protected override Task<bool> IsPermittedAsync(RequestInfo request, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        protected override RequestState GetRequestState(bool isPermitted, RequestState currentState)
        {
            return currentState != RequestState.Accepted ? RequestState.Denied : RequestState.Accepted;
        }
    }
}
