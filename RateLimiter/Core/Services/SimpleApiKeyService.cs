using Microsoft.Extensions.Logging;
using RateLimiter.Common.Abstractions;
using RateLimiter.Common.Models;

namespace RateLimiter.Core.Services;

/// <summary>
/// Simple in-memory API key validation service for demonstration.
/// In production, this should integrate with your API key management system.
/// </summary>
public class SimpleApiKeyService : IAuthenticationService
{
    private readonly ILogger<SimpleApiKeyService> _logger;
    private readonly Dictionary<string, ApiClient> _apiKeys;

    public SimpleApiKeyService(ILogger<SimpleApiKeyService> logger)
    {
        _logger = logger;
        
        // Sample API keys for demonstration
        _apiKeys = new Dictionary<string, ApiClient>
        {
            ["demo-api-key-123"] = new ApiClient
            {
                ClientId = "demo-client-1",
                Name = "Demo Client 1",
                Region = "US",
                Tier = "standard",
                IsActive = true
            },
            ["premium-api-key-456"] = new ApiClient
            {
                ClientId = "premium-client-1",
                Name = "Premium Client 1",
                Region = "EU",
                Tier = "premium",
                IsActive = true
            }
        };
    }

    /// <summary>
    /// Simple API key validation (not implemented - use JwtAuthenticationService for JWT).
    /// </summary>
    public Task<AuthenticatedUser?> ValidateJwtTokenAsync(string token)
    {
        // This service only handles API keys
        return Task.FromResult<AuthenticatedUser?>(null);
    }

    /// <summary>
    /// Validates an API key and returns client information.
    /// </summary>
    public Task<ApiClient?> ValidateApiKeyAsync(string apiKey)
    {
        if (string.IsNullOrEmpty(apiKey))
        {
            return Task.FromResult<ApiClient?>(null);
        }

        if (_apiKeys.TryGetValue(apiKey, out var client) && client.IsActive)
        {
            _logger.LogDebug("API key validated for client {ClientId}", client.ClientId);
            return Task.FromResult<ApiClient?>(client);
        }

        _logger.LogWarning("Invalid or inactive API key attempted: {ApiKey}", apiKey);
        return Task.FromResult<ApiClient?>(null);
    }
}
