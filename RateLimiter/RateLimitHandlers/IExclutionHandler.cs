using RateLimiter.Enums;
using RateLimiter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.RateLimitHandlers
{
    public interface IRateLimitHandler<TClientIdentity, TResponse>
    {
        ERoleType RoleType { get; }

        IRateLimitHandler<TClientIdentity, TResponse> SetNext(IRateLimitHandler<TClientIdentity, TResponse> handler);

        Task<TResponse> CheckLimit(TClientIdentity client);
    }
}
