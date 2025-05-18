namespace RateLimiter.Common.Models;

/// <summary>
/// Represents geographic location information for an IP address.
/// </summary>
public class GeoLocation
{
    /// <summary>
    /// Gets or sets the country code (e.g., "US", "GB").
    /// </summary>
    public string? CountryCode { get; set; }
    
    /// <summary>
    /// Gets or sets the country name.
    /// </summary>
    public string? CountryName { get; set; }
    
    /// <summary>
    /// Gets or sets the region/state code.
    /// </summary>
    public string? RegionCode { get; set; }
    
    /// <summary>
    /// Gets or sets the region/state name.
    /// </summary>
    public string? RegionName { get; set; }
    
    /// <summary>
    /// Gets or sets the city name.
    /// </summary>
    public string? City { get; set; }
    
    /// <summary>
    /// Gets or sets the postal code.
    /// </summary>
    public string? PostalCode { get; set; }
    
    /// <summary>
    /// Gets or sets the latitude.
    /// </summary>
    public double? Latitude { get; set; }
    
    /// <summary>
    /// Gets or sets the longitude.
    /// </summary>
    public double? Longitude { get; set; }
    
    /// <summary>
    /// Gets or sets the timezone.
    /// </summary>
    public string? TimeZone { get; set; }
}
