using System.Text.Json;
using Microsoft.Extensions.Options;
using RateLimiter.Common.Abstractions.Rules;
using RateLimiter.Common.Models;
using RateLimiter.Core.Configuration;

namespace RateLimiter.Api.Middleware;

/// <summary>
/// Middleware for applying rate limiting to HTTP requests.
/// </summary>
public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IRateLimiterService _rateLimiter;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    private readonly RateLimitOptions _options;

    public RateLimitingMiddleware(
        RequestDelegate next,
        IRateLimiterService rateLimiter,
        ILogger<RateLimitingMiddleware> logger,
        IOptions<RateLimitOptions> options)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _rateLimiter = rateLimiter ?? throw new ArgumentNullException(nameof(rateLimiter));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// Processes the request through the rate limiting middleware.
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        // Skip rate limiting if disabled
        if (!_options.EnableRateLimiting)
        {
            _logger.LogDebug("Rate limiting is disabled. Skipping middleware.");
            await _next(context);
            return;
        }
        
        _logger.LogDebug(
            "Evaluating rate limits for request {Method} {Path}", 
            context.Request.Method, context.Request.Path);
        
        // Evaluate rate limits
        RateLimitResult result;
        try
        {
            result = await _rateLimiter.EvaluateRequestAsync(context);
            
            _logger.LogDebug(
                "Rate limit evaluation result: {IsAllowed}, Rule: {Rule}, Counter: {Counter}, Limit: {Limit}", 
                result.IsAllowed, result.Rule, result.Counter, result.Limit);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error evaluating rate limit");
            
            // Fail open - allow the request to proceed
            await _next(context);
            return;
        }
        
        // If headers are enabled, add rate limit info headers
        if (_options.IncludeHeaders)
        {
            AddRateLimitHeaders(context, result);
        }
        
        // If request is allowed, proceed to next middleware
        if (result.IsAllowed)
        {
            _logger.LogDebug("Request allowed by rate limit: {Rule}", result.Rule);
            await _next(context);
            return;
        }
        
        // Request is blocked - return 429 Too Many Requests
        _logger.LogInformation(
            "Request blocked by rate limit: {Rule}. Current: {Counter}, Limit: {Limit}",
            result.Rule, result.Counter, result.Limit);
            
        // Set appropriate response for rate limited request
        context.Response.StatusCode = _options.StatusCode;
        context.Response.ContentType = "application/json";
        
        // Add Retry-After header if reset time is available
        if (result.ResetAfter.HasValue)
        {
            context.Response.Headers.Append(
                "Retry-After",
                ((int)Math.Ceiling(result.ResetAfter.Value.TotalSeconds)).ToString());
        }
        
        // Return error message with retry information
        string message = $"Rate limit exceeded. Try again in {GetHumanReadableTimeSpan(result.ResetAfter ?? TimeSpan.FromMinutes(1))}.";
        if (!string.IsNullOrEmpty(result.Message))
        {
            message += $" {result.Message}";
        }
        
        var response = new
        {
            error = "Too many requests",
            message = message,
            rule = result.Rule,
            limit = result.Limit,
            resetAfter = result.ResetAfter?.TotalSeconds
        };
        
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
    
    /// <summary>
    /// Adds rate limit headers to the response.
    /// </summary>
    private void AddRateLimitHeaders(HttpContext context, RateLimitResult result)
    {
        if (result == null)
        {
            return;
        }
        
        string prefix = _options.HeaderPrefix;
        
        // Add basic rate limit headers
        context.Response.Headers.Append($"{prefix}-Limit", result.Limit.ToString());
        context.Response.Headers.Append($"{prefix}-Remaining", Math.Max(0, result.Limit - result.Counter).ToString());
        
        // Add window size
        context.Response.Headers.Append($"{prefix}-Window", result.TimeWindowInSeconds.ToString());
        
        // Add reset time if available
        if (result.ResetAfter.HasValue)
        {
            context.Response.Headers.Append(
                $"{prefix}-Reset",
                ((int)Math.Ceiling(result.ResetAfter.Value.TotalSeconds)).ToString());
        }
        
        // Add rule name
        context.Response.Headers.Append($"{prefix}-Rule", result.Rule);
    }
    
    /// <summary>
    /// Converts a TimeSpan to a human-readable string.
    /// </summary>
    private string GetHumanReadableTimeSpan(TimeSpan timeSpan)
    {
        if (timeSpan.TotalSeconds < 1)
        {
            return "less than a second";
        }
        if (timeSpan.TotalSeconds < 60)
        {
            return $"{(int)timeSpan.TotalSeconds} second{(timeSpan.TotalSeconds == 1 ? "" : "s")}";
        }
        if (timeSpan.TotalMinutes < 60)
        {
            return $"{(int)timeSpan.TotalMinutes} minute{(timeSpan.TotalMinutes == 1 ? "" : "s")}";
        }
        
        return $"{(int)timeSpan.TotalHours} hour{(timeSpan.TotalHours == 1 ? "" : "s")}";
    }
}
