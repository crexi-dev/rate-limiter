using RateLimiter.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Interfaces
{
    public interface IMiddleware
    {
        Task InvokeAsync(RateLimiterHandlerContext context);
    }
}
