using RateLimiter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.RateLimitHandlers
{
    public abstract class ProcessFactoryBase
    {
        #region TO DO - Make Abstract
        private readonly XRequestsPerTimespanHandler _xRequestsPerTimespan;
        private readonly LastCallHandler _lastCallHandler;
        private readonly USBasedTokensHandler _usBasedTokensHandler;
        private readonly EUBasedTokensHandler _eUBasedTokensHandler;
        #endregion

        public ProcessFactoryBase(XRequestsPerTimespanHandler xRequestsPerTimespan, 
                                  LastCallHandler lastCallHandler,
                                  USBasedTokensHandler usBasedTokensHandler,
                                  EUBasedTokensHandler eUBasedTokensHandler)
        {
            _xRequestsPerTimespan = xRequestsPerTimespan;
            _lastCallHandler = lastCallHandler;
            _usBasedTokensHandler = usBasedTokensHandler;
            _eUBasedTokensHandler = eUBasedTokensHandler;
        }

        protected internal virtual IRateLimitHandler<TClientIdentity, TResponse> CrateChain<TRateLimitHandler, TClientIdentity, TResponse>()
            where TRateLimitHandler : IRateLimitHandler<TClientIdentity, TResponse>
            where TClientIdentity : IClientRequestIdentity
            where TResponse : IQuotaResponse
        {
            _xRequestsPerTimespan.SetNext(_lastCallHandler)
                                 .SetNext(_usBasedTokensHandler)
                                 .SetNext(_eUBasedTokensHandler);
            return (IRateLimitHandler<TClientIdentity, TResponse>)_xRequestsPerTimespan;
        }

        public virtual async Task<IQuotaResponse> Check<TClientIdentity>(TClientIdentity client) 
            where TClientIdentity : IClientRequestIdentity 
        {
            var chain = CrateChain<IRateLimitHandler<IClientRequestIdentity, IQuotaResponse>, IClientRequestIdentity, IQuotaResponse> ();
            var res = await chain.CheckLimit(client);

            return res;
        }
    }
}
