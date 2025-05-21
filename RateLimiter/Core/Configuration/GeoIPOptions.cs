namespace RateLimiter.Core.Configuration;

/// <summary>
/// Configuration options for GeoIP services.
/// </summary>
public class GeoIPOptions
{
    /// <summary>
    /// Gets or sets the path to the MaxMind GeoIP database file.
    /// </summary>
    public string? DatabasePath { get; set; }
    
    /// <summary>
    /// Gets or sets the default region for unknown locations.
    /// </summary>
    public string DefaultRegion { get; set; } = "UNKNOWN";
    
    /// <summary>
    /// Gets or sets whether GeoIP lookup is enabled.
    /// </summary>
    public bool Enabled { get; set; } = false;
}
