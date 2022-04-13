using Microsoft.Extensions.Configuration;
using RateLimiter.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RateLimiter.Rules
{
    /// <summary>
    /// The base class for all rules
    /// </summary>
    public abstract class RateLimiterBaseRule
    {
        /// <summary>
        /// Returns whether a particular request can be analyzed by this rule
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected abstract Task<bool> IsApplicableAsync(RequestInfo request, CancellationToken cancellationToken);

        /// <summary>
        /// Execute the rule logic to determine whether the Request is permitted under the Rate Limiting logic
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected abstract Task<bool> IsPermittedAsync(RequestInfo request, CancellationToken cancellationToken);

        /// <summary>
        /// Calculate a unique identifier for a given <see cref="Token"/>. The rule will limit the rate of requests
        /// with the same Identifier.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        protected virtual string GetIdForToken(Token token)
        {
            return token.Subject;
        }

        protected virtual RequestState GetRequestState(bool isPermitted, RequestState currentState)
        {
            if (!isPermitted)
                return RequestState.Denied;
            return currentState == RequestState.Denied ? RequestState.Denied : RequestState.Accepted;
        }

        /// <summary>
        /// Attempt to apply the rule to the request, and return the new RequestState after processing. If the rule 
        /// cannot handle the request, then the current request state is returned. 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="currentState">The state of  the request before the rule executes</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<RequestState> GetRequestStateAsync(RequestInfo request, RequestState currentState, CancellationToken cancellationToken)
        {
            if (!await this.IsApplicableAsync(request, cancellationToken))
                return currentState;
            cancellationToken.ThrowIfCancellationRequested();
            var isPermitted = await this.IsPermittedAsync(request, cancellationToken);
            return this.GetRequestState(isPermitted, currentState);
        }
    }
}
