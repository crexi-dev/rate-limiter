using Microsoft.AspNetCore.Http;
using RateLimiter.Nugget.Interfaces;

namespace RateLimiter.Nugget.Services
{
    public class RateLimiter<T>
    {
        public bool IsRateLimitExceeded(HttpContext httpContext)
        {
            string routeUrl = httpContext.Request.Path;
            string accessToken = httpContext.Request.Headers["accessToken"];

            if (!SharedRoutes.GetRoutes()
                    .Any(pair => pair.Value.Contains(routeUrl.ToLower())))
            {
                return false;
            }

            foreach (var rule in SharedRoutes.GetLimiterRuleTypeByRoute(routeUrl.ToLower()))
            {
                IRateLimitRule<T> instance = (IRateLimitRule<T>)Activator.CreateInstance(rule);
                if (instance.IsRateLimitExceeded(accessToken))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
