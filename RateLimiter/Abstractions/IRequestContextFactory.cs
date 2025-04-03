using Microsoft.AspNetCore.Http;
using RateLimiter.Models;

namespace RateLimiter.Abstractions
{
    public interface IRequestContextFactory
    {
        RequestContext Create(HttpContext httpContext);
    }
}