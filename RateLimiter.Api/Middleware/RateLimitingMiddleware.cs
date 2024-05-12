using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using RateLimiter.Api.Models;

namespace RateLimiter.Api.Middleware;

public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly RateLimitingOptions _options;
    private static readonly ConcurrentDictionary<string, List<DateTime>> _requests = new();

    public RateLimitingMiddleware(RequestDelegate next, IOptions<RateLimitingOptions> options)
    {
        _next = next;
        _options = options.Value;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].ToString();
        var country = context.Request.Headers["X-Country"].ToString();
        
        if (string.IsNullOrEmpty(token))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Unauthorized");
            return;
        }

        if (!IsRequestAllowed(token, country))
        {
            context.Response.StatusCode = 429;
            await context.Response.WriteAsync("Too Many Requests");
            return;
        }

        await _next(context);
    }

     private bool IsRequestAllowed(string token, string country)
        {
            var now = DateTime.UtcNow;

            var requestTimes = _requests.GetOrAdd(token, _ => new List<DateTime>());

            lock (requestTimes)
            {
                var countryRule = _options.CountrySpecificRules
                    .FirstOrDefault(rule => rule.Country.Equals(country, StringComparison.OrdinalIgnoreCase));

                if (countryRule != null)
                {
                    if (countryRule.TimeBetweenRequestsSeconds.HasValue)
                    {
                        var minAllowedTime = now.AddSeconds(-countryRule.TimeBetweenRequestsSeconds.Value);
                        if (requestTimes.Any(t => t > minAllowedTime))
                        {
                            return false;
                        }
                    }

                    if (countryRule.MaxRequests.HasValue)
                    {
                        var timeWindowStart = now.AddSeconds(-_options.GeneralRules.First().TimeSpanSeconds);
                        if (requestTimes.Count(t => t > timeWindowStart) >= countryRule.MaxRequests)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    foreach (var rule in _options.GeneralRules)
                    {
                        var timeWindowStart = now.AddSeconds(-rule.TimeSpanSeconds);
                        if (requestTimes.Count(t => t > timeWindowStart) >= rule.MaxRequests)
                        {
                            return false;
                        }
                    }
                }

                requestTimes.Add(now);
                _requests[token] = requestTimes
                    .Where(t => t > now.AddSeconds(-_options.GeneralRules.Max(r => r.TimeSpanSeconds)))
                    .ToList();
            }

            return true;
        }
}
