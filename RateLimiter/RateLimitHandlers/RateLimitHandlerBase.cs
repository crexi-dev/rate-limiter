using RateLimiter.Enums;
using RateLimiter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.RateLimitHandlers
{
    public abstract class RateLimitHandlerBase<TClientIdentity, TResponse> : IRateLimitHandler<TClientIdentity, TResponse> 
        where TClientIdentity : IClientRequestIdentity
        where TResponse : IQuotaResponse, new()
    {
        private IRateLimitHandler<TClientIdentity, TResponse> _nextHandler;

        public abstract ERoleType RoleType { get; init;}

        public abstract Task<TResponse> CheckLimit(TClientIdentity client);

        public virtual IRateLimitHandler<TClientIdentity, TResponse> SetNext(IRateLimitHandler<TClientIdentity, TResponse> handler)
        {
            _nextHandler = handler;
            return _nextHandler;
        }

        public virtual async Task<TResponse> ToNext(TClientIdentity client)
        {
            if (_nextHandler != null)
                return await _nextHandler.CheckLimit(client);

            return new TResponse() { Passed = true };
        }
    }
}
