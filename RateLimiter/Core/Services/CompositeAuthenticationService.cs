using Microsoft.Extensions.Logging;
using RateLimiter.Common.Abstractions;
using RateLimiter.Common.Models;

namespace RateLimiter.Core.Services;

/// <summary>
/// Composite authentication service that combines JWT and API key authentication.
/// </summary>
public class CompositeAuthenticationService : IAuthenticationService
{
    private readonly IEnumerable<IAuthenticationService> _authServices;
    private readonly ILogger<CompositeAuthenticationService> _logger;

    public CompositeAuthenticationService(
        IEnumerable<IAuthenticationService> authServices,
        ILogger<CompositeAuthenticationService> logger)
    {
        _authServices = authServices ?? throw new ArgumentNullException(nameof(authServices));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Validates a JWT token using all available authentication services.
    /// </summary>
    public async Task<AuthenticatedUser?> ValidateJwtTokenAsync(string token)
    {
        foreach (var service in _authServices)
        {
            try
            {
                var result = await service.ValidateJwtTokenAsync(token);
                if (result != null)
                {
                    _logger.LogDebug("JWT token validated by {ServiceType}", service.GetType().Name);
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error in JWT validation service {ServiceType}", service.GetType().Name);
            }
        }

        return null;
    }

    /// <summary>
    /// Validates an API key using all available authentication services.
    /// </summary>
    public async Task<ApiClient?> ValidateApiKeyAsync(string apiKey)
    {
        foreach (var service in _authServices)
        {
            try
            {
                var result = await service.ValidateApiKeyAsync(apiKey);
                if (result != null)
                {
                    _logger.LogDebug("API key validated by {ServiceType}", service.GetType().Name);
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error in API key validation service {ServiceType}", service.GetType().Name);
            }
        }

        return null;
    }
}
