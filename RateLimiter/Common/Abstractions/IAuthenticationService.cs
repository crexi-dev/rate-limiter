using RateLimiter.Common.Models;

namespace RateLimiter.Common.Abstractions;

/// <summary>
/// Service for validating authentication tokens and extracting user information.
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Validates a JWT token and extracts user information.
    /// </summary>
    /// <param name="token">The JWT token to validate</param>
    /// <returns>Authenticated user information, or null if invalid</returns>
    Task<AuthenticatedUser?> ValidateJwtTokenAsync(string token);
    
    /// <summary>
    /// Validates an API key and extracts client information.
    /// </summary>
    /// <param name="apiKey">The API key to validate</param>
    /// <returns>API client information, or null if invalid</returns>
    Task<ApiClient?> ValidateApiKeyAsync(string apiKey);
}
