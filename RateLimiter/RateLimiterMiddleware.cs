using Microsoft.AspNetCore.Http;
using RateLimiter.Handlers;
using RateLimiter.Statistics;
using System.Net;

namespace RateLimiter
{
    internal class RateLimiterMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IStatisticsService _statisticsService;
        private readonly IRateLimiterHandlerResolver _handlerResolver;

        public RateLimiterMiddleware(RequestDelegate next, IStatisticsService statisticsService, IRateLimiterHandlerResolver handlerResolver)
        {
            _next = next;
            _statisticsService = statisticsService;
            _handlerResolver = handlerResolver;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            var rateLimiterOptions = endpoint?.Metadata.GetMetadata<RateLimiterAttribute>();
            if (rateLimiterOptions is null)
            {
                await _next(context);
                return;
            }

            var key = GetClientToken(context);

            var clientStatistics = await _statisticsService.GetClientStatistics<ClientStatistics>(key);

            if (clientStatistics is null)
                clientStatistics = new ClientStatistics();

            var handler = _handlerResolver.Resolve(key);

            if (handler.IsExceeded(clientStatistics, rateLimiterOptions))
            {
                context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                return;
            }

            clientStatistics.NumberOfRequests++;
            clientStatistics.LastSuccessfulResponseTime = DateTime.UtcNow;

            await _statisticsService.UpdateClientStatistics(key, clientStatistics);
            await _next(context);
        }

        private static string GetClientToken(HttpContext context)
        {
            return context.Request.Headers.TryGetValue("Authorization", out var value)
                ? value.ToString().Split("Bearer ")[1]
                : string.Empty;
        }
    }
}
