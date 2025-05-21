namespace RateLimiter.Core.Configuration;

/// <summary>
/// Configuration options for rate limiting.
/// </summary>
public class RateLimitOptions
{
    /// <summary>
    /// Gets or sets whether rate limiting is enabled.
    /// </summary>
    public bool EnableRateLimiting { get; set; } = true;
    
    /// <summary>
    /// Gets or sets whether to include rate limit headers in responses.
    /// </summary>
    public bool IncludeHeaders { get; set; } = true;
    
    /// <summary>
    /// Gets or sets the prefix for rate limit headers.
    /// </summary>
    public string HeaderPrefix { get; set; } = "X-RateLimit";
    
    /// <summary>
    /// Gets or sets the HTTP status code to return when rate limit is exceeded.
    /// </summary>
    public int StatusCode { get; set; } = 429;
    
    /// <summary>
    /// Gets or sets the client ID header name.
    /// </summary>
    public string? ClientIdHeaderName { get; set; } = "X-ClientId";
    
    /// <summary>
    /// Gets or sets the region header name.
    /// </summary>
    public string? RegionHeaderName { get; set; } = "X-Region";
}
