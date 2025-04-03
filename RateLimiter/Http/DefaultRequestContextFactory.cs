using System;
using Microsoft.AspNetCore.Http;
using RateLimiter.Abstractions;
using RateLimiter.Models;

namespace RateLimiter.Http
{
    public class DefaultRequestContextFactory : IRequestContextFactory
    {
        private const string DefaultRegion = "GLOBAL";
        private const string RegionHeaderName = "X-Region";
        private const string TokenHeaderName = "Authorization";
        
        public RequestContext Create(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }
            
            var context = new RequestContext
            {
                ResourcePath = httpContext.Request.Path.Value ?? string.Empty,
                Timestamp = DateTime.UtcNow
            };
            
            if (httpContext.Request.Headers.TryGetValue(TokenHeaderName, out var authHeader))
            {
                var authValue = authHeader.ToString();
                
                if (authValue.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    context.ClientToken = authValue.Substring(7).Trim();
                }
                else
                {
                    context.ClientToken = authValue.Trim();
                }
            }
            
            if (httpContext.Request.Headers.TryGetValue(RegionHeaderName, out var regionHeader))
            {
                context.Region = regionHeader.ToString().Trim();
            }
            else
            {
                context.Region = DefaultRegion;
            }
            
            return context;
        }
    }
}