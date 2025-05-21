using RateLimiter.Common.Models;

namespace RateLimiter.Common.Abstractions;

/// <summary>
/// Service for IP geolocation and region determination.
/// </summary>
public interface IGeoIPService
{
    /// <summary>
    /// Gets geographic information for an IP address.
    /// </summary>
    /// <param name="ipAddress">The IP address to locate</param>
    /// <returns>Geographic information, or null if unavailable</returns>
    Task<GeoLocation?> GetLocationAsync(string ipAddress);
    
    /// <summary>
    /// Gets the business region for an IP address.
    /// </summary>
    /// <param name="ipAddress">The IP address to locate</param>
    /// <returns>Business region code (e.g., "US", "EU", "APAC")</returns>
    Task<string> GetRegionAsync(string ipAddress);
}
