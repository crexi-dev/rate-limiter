using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RateLimiter.Common.Abstractions;
using RateLimiter.Common.Models;
using RateLimiter.Core.Configuration;

namespace RateLimiter.Core.Services;

/// <summary>
/// JWT-based authentication service implementation.
/// </summary>
public class JwtAuthenticationService : IAuthenticationService
{
    private readonly JwtAuthenticationOptions _options;
    private readonly ILogger<JwtAuthenticationService> _logger;
    private readonly JwtSecurityTokenHandler _tokenHandler;

    public JwtAuthenticationService(
        IOptions<JwtAuthenticationOptions> options,
        ILogger<JwtAuthenticationService> logger)
    {
        _options = options.Value;
        _logger = logger;
        _tokenHandler = new JwtSecurityTokenHandler();
    }

    /// <summary>
    /// Validates a JWT token and extracts user information.
    /// </summary>
    public async Task<AuthenticatedUser?> ValidateJwtTokenAsync(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return null;
        }

        try
        {
            // Remove "Bearer " prefix if present
            if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                token = token.Substring(7);
            }

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_options.SecretKey)),
                ValidateIssuer = !string.IsNullOrEmpty(_options.Issuer),
                ValidIssuer = _options.Issuer,
                ValidateAudience = !string.IsNullOrEmpty(_options.Audience),
                ValidAudience = _options.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(5)
            };

            var principal = _tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);

            if (validatedToken is not JwtSecurityToken jwtToken)
            {
                _logger.LogWarning("Invalid JWT token format");
                return null;
            }

            return await Task.FromResult(ExtractUserFromClaims(principal.Claims));
        }
        catch (SecurityTokenException ex)
        {
            _logger.LogWarning(ex, "JWT token validation failed");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during JWT token validation");
            return null;
        }
    }

    /// <summary>
    /// Validates an API key (not implemented in JWT service).
    /// </summary>
    public Task<ApiClient?> ValidateApiKeyAsync(string apiKey)
    {
        // JWT service doesn't handle API keys
        return Task.FromResult<ApiClient?>(null);
    }

    private AuthenticatedUser ExtractUserFromClaims(IEnumerable<Claim> claims)
    {
        var claimsList = claims.ToList();
        var user = new AuthenticatedUser();

        foreach (var claim in claimsList)
        {
            switch (claim.Type)
            {
                case ClaimTypes.NameIdentifier:
                case "sub":
                    user.UserId = claim.Value;
                    break;
                case ClaimTypes.Email:
                case "email":
                    user.Email = claim.Value;
                    break;
                case "region":
                    user.Region = claim.Value;
                    break;
                case "tier":
                    user.Tier = claim.Value;
                    break;
                default:
                    user.Claims[claim.Type] = claim.Value;
                    break;
            }
        }

        return user;
    }
}
