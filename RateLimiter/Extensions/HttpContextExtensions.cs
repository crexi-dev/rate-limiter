using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace RateLimiter.Extensions
{
    public static class HttpContextExtensions
    {
        public static string GetClientId(this HttpContext httpContext)
        {
            return httpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
