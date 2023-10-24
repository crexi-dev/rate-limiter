using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using RateLimiter.Models;
using RateLimiter.RateLimitHandlers;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Middlewares
{
    public abstract class RateLimitMiddlewareBase<TRateChecker> : IMiddleware
        where TRateChecker : ProcessFactoryBase
    {
        private readonly RequestDelegate _next;
        private readonly ProcessFactoryBase _processFactory;
        private readonly RateLimitOptions _options;

        protected RateLimitMiddlewareBase(RequestDelegate next, IOptions<RateLimitOptions> options, ProcessFactoryBase processFactory)
        {
            _next = next;
            _processFactory = processFactory;
            _options = options.Value;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (_options == null)
            {
                await _next.Invoke(context);
                return;
            }

            var client = await ResolveIdentityAsync(context);

            var res = await _processFactory.Check(client);

            if (res.Passed)
            {
                await _next.Invoke(context);
                return;
            }

            await ReturnQuotaExceededResponse(context, res);
            return;
        }

        public virtual async Task<ClientRequestIdentity> ResolveIdentityAsync(HttpContext httpContext)
        {
            var token = await httpContext.GetTokenAsync(_options.AccessTokenName);
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenData = tokenHandler.ReadJwtToken(token);

            var clientId = tokenData.Claims.FirstOrDefault(x => x.Type == _options.ClientIdName)?.Value;
            var regionPrefix = tokenData.Claims.FirstOrDefault(x => x.Type == _options.TokenRegionName)?.Value;

            return new ClientRequestIdentity { ClientId = clientId, RegionPrefix = regionPrefix };
        }

        public virtual Task ReturnQuotaExceededResponse(HttpContext httpContext, IQuotaResponse response)
        {
            httpContext.Response.Headers["Retry-After"] = response.RetryAfter;
            httpContext.Response.StatusCode = response.StatusCode.Value;
            httpContext.Response.ContentType = response.ContentType;

            return httpContext.Response.WriteAsync(response.Message);
        }
    }
}
