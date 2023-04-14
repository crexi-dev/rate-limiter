using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using RateLimiter.Stores.Interfaces;

namespace RateLimiter.Middlewares;

public class RequestLoggerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IRateLimitStore _storage;
    
    public RequestLoggerMiddleware(RequestDelegate next, IRateLimitStore storage)
    {
        _next = next;
        _storage = storage;
    }

    public async Task Invoke(HttpContext context)
    {
        await _storage.Add(context.Request.Path);
        await _next(context);
    }
}