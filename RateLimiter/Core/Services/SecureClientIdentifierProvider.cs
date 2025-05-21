using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RateLimiter.Common.Abstractions;
using RateLimiter.Common.Models;
using RateLimiter.Core.Configuration;

namespace RateLimiter.Core.Services;

/// <summary>
/// Secure client identifier provider that uses server-side authentication and GeoIP.
/// </summary>
public class SecureClientIdentifierProvider : IRateLimitClientIdentifierProvider
{
    private readonly RateLimitOptions _options;
    private readonly IAuthenticationService _authService;
    private readonly IGeoIPService _geoIPService;
    private readonly ILogger<SecureClientIdentifierProvider> _logger;

    public SecureClientIdentifierProvider(
        IOptions<RateLimitOptions> options,
        IAuthenticationService authService,
        IGeoIPService geoIPService,
        ILogger<SecureClientIdentifierProvider> logger)
    {
        _options = options.Value;
        _authService = authService;
        _geoIPService = geoIPService;
        _logger = logger;
    }

    /// <summary>
    /// Gets a secure client identifier from the HTTP context.
    /// </summary>
    public async Task<ClientIdentifier> GetClientIdentifierAsync(HttpContext context)
    {
        var identifier = new ClientIdentifier();
        
        // Try to get authenticated user information
        var authenticatedUser = await TryGetAuthenticatedUser(context);
        if (authenticatedUser != null)
        {
            identifier.Id = authenticatedUser.UserId;
            identifier.Region = authenticatedUser.Region;
            identifier.Attributes["tier"] = authenticatedUser.Tier ?? "standard";
            identifier.Attributes["email"] = authenticatedUser.Email ?? "";
            
            // Add all custom claims
            foreach (var claim in authenticatedUser.Claims)
            {
                identifier.Attributes[$"claim_{claim.Key}"] = claim.Value;
            }
            
            _logger.LogDebug("Authenticated user identified: {UserId}", authenticatedUser.UserId);
        }
        else
        {
            // Try to get API client information
            var apiClient = await TryGetApiClient(context);
            if (apiClient != null)
            {
                identifier.Id = apiClient.ClientId;
                identifier.Region = apiClient.Region;
                identifier.Attributes["tier"] = apiClient.Tier ?? "standard";
                identifier.Attributes["name"] = apiClient.Name ?? "";
                
                // Add all custom attributes
                foreach (var attr in apiClient.Attributes)
                {
                    identifier.Attributes[attr.Key] = attr.Value;
                }
                
                _logger.LogDebug("API client identified: {ClientId}", apiClient.ClientId);
            }
        }
        
        // Set IP address
        identifier.IpAddress = context.Connection.RemoteIpAddress?.ToString();
        
        // If no authenticated identity found, use IP-based identification
        if (string.IsNullOrEmpty(identifier.Id))
        {
            identifier.Id = identifier.IpAddress ?? "unknown";
            _logger.LogDebug("Using IP-based identification: {IpAddress}", identifier.IpAddress);
        }
        
        // Determine region using GeoIP if not already set
        if (string.IsNullOrEmpty(identifier.Region) && !string.IsNullOrEmpty(identifier.IpAddress))
        {
            try
            {
                identifier.Region = await _geoIPService.GetRegionAsync(identifier.IpAddress);
                _logger.LogDebug("GeoIP region determined: {Region} for IP {IpAddress}", 
                    identifier.Region, identifier.IpAddress);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to determine region from GeoIP for {IpAddress}", identifier.IpAddress);
                identifier.Region = "UNKNOWN";
            }
        }
        
        // Validate claimed region against GeoIP (if header provided)
        await ValidateClaimedRegion(context, identifier);
        
        // Set additional context
        identifier.Attributes["user_agent"] = context.Request.Headers.UserAgent.ToString();
        identifier.Attributes["accept_language"] = context.Request.Headers.AcceptLanguage.ToString();
        
        return identifier;
    }

    private async Task<AuthenticatedUser?> TryGetAuthenticatedUser(HttpContext context)
    {
        try
        {
            // Try JWT token from Authorization header
            if (context.Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                var token = authHeader.FirstOrDefault();
                if (!string.IsNullOrEmpty(token))
                {
                    var user = await _authService.ValidateJwtTokenAsync(token);
                    if (user != null)
                    {
                        return user;
                    }
                }
            }
            
            // Try from authenticated user context (if middleware already processed)
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var userId = context.User.FindFirst("sub")?.Value ?? 
                           context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                
                if (!string.IsNullOrEmpty(userId))
                {
                    return new AuthenticatedUser
                    {
                        UserId = userId,
                        Region = context.User.FindFirst("region")?.Value,
                        Tier = context.User.FindFirst("tier")?.Value,
                        Email = context.User.FindFirst("email")?.Value ??
                               context.User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
                    };
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error trying to get authenticated user");
        }
        
        return null;
    }

    private async Task<ApiClient?> TryGetApiClient(HttpContext context)
    {
        try
        {
            // Try API key from X-API-Key header
            if (context.Request.Headers.TryGetValue("X-API-Key", out var apiKeyHeader))
            {
                var apiKey = apiKeyHeader.FirstOrDefault();
                if (!string.IsNullOrEmpty(apiKey))
                {
                    return await _authService.ValidateApiKeyAsync(apiKey);
                }
            }
            
            // Try API key from query parameter
            if (context.Request.Query.TryGetValue("api_key", out var apiKeyQuery))
            {
                var apiKey = apiKeyQuery.FirstOrDefault();
                if (!string.IsNullOrEmpty(apiKey))
                {
                    return await _authService.ValidateApiKeyAsync(apiKey);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error trying to get API client");
        }
        
        return null;
    }

    private async Task ValidateClaimedRegion(HttpContext context, ClientIdentifier identifier)
    {
        if (!context.Request.Headers.TryGetValue(_options.RegionHeaderName ?? "X-Region", out var regionHeader))
        {
            return;
        }
        
        var claimedRegion = regionHeader.FirstOrDefault();
        if (string.IsNullOrEmpty(claimedRegion) || string.IsNullOrEmpty(identifier.IpAddress))
        {
            return;
        }
        
        try
        {
            var geoRegion = await _geoIPService.GetRegionAsync(identifier.IpAddress);
            
            if (!IsValidRegionClaim(claimedRegion, geoRegion))
            {
                _logger.LogWarning(
                    "Suspicious region claim: {ClaimedRegion} vs GeoIP {GeoRegion} from IP {IpAddress}",
                    claimedRegion, geoRegion, identifier.IpAddress);
                    
                // Flag as suspicious but don't override - use GeoIP region
                identifier.Attributes["suspicious_region_claim"] = "true";
                identifier.Attributes["claimed_region"] = claimedRegion;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error validating claimed region {ClaimedRegion}", claimedRegion);
        }
    }

    private static bool IsValidRegionClaim(string claimed, string geoIP)
    {
        if (claimed == geoIP)
        {
            return true;
        }
        
        // Allow some reasonable mappings
        var validMappings = new Dictionary<string, string[]>
        {
            ["NA"] = new[] { "US", "CA", "MX" },
            ["EU"] = new[] { "GB", "DE", "FR", "IT", "ES", "NL", "BE", "AT", "CH" },
            ["APAC"] = new[] { "JP", "KR", "SG", "HK", "TW", "AU", "NZ" }
        };
        
        return validMappings.ContainsKey(claimed) && validMappings[claimed].Contains(geoIP);
    }
}
