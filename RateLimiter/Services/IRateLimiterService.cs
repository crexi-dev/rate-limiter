namespace RateLimiter.Services;

public interface IRateLimiterService
{
    /// <summary>
    /// Determines whether a request is allowed based on the endpoint and token.
    /// </summary>
    /// <param name="endpoint">The endpoint being accessed.</param>
    /// <param name="token">The token representing the client making the request.</param>
    /// <returns>True if the request is allowed; otherwise, false.</returns>
    bool IsRequestAllowed(string endpoint, string token);
}