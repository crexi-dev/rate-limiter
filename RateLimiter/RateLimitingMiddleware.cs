using System.Collections.Concurrent;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using RateLimiter.Models;
using RateLimiter.Services;

namespace RateLimiter;

public class RateLimitingMiddleware
{
    private static readonly ConcurrentDictionary<string, ClientData> Clients =  new();
    private static readonly object SyncLock = new();
    private readonly RequestDelegate _next;
    public RateLimitingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IRateLimitValidatorFactory rateLimitValidatorFactory)
    {
        var clientId = context.Connection.RemoteIpAddress.ToString();
        var regionKey = GetRegionKey(context.User);
        lock (SyncLock)
        {
            var clientData = Clients.GetOrAdd(clientId, ClientData.Empty);
            var validator = rateLimitValidatorFactory.Create(regionKey);
            var result = validator.Validate(clientData);
            if (!result.Result)
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                return;
            }

            if (result.VisitCounts.HasValue)
            {
                clientData.VisitCounts = result.VisitCounts.Value;
            }

            Clients[clientId] = clientData;
        }

        await _next(context);
    }

    private static string? GetRegionKey(ClaimsPrincipal user)
    {
        var regionKeyClaim = user.Claims.FirstOrDefault(c => c.Type == "RegionKey");
        return regionKeyClaim?.Value;
    }
}