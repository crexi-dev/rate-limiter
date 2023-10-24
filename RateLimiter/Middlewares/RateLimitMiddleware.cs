using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using RateLimiter.RateLimitHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Middlewares
{
    public class RateLimitMiddleware : RateLimitMiddlewareBase<ProcessFactory>
    {
        public RateLimitMiddleware(RequestDelegate next, IOptions<RateLimitOptions> options, ProcessFactoryBase processFactory) : base(next, options, processFactory)
        {
        }
    }
}
