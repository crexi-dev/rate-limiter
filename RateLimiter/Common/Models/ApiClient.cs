namespace RateLimiter.Common.Models;

/// <summary>
/// Represents an API client identified by API key.
/// </summary>
public class ApiClient
{
    /// <summary>
    /// Gets or sets the client ID.
    /// </summary>
    public string ClientId { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the client's registered region.
    /// </summary>
    public string? Region { get; set; }
    
    /// <summary>
    /// Gets or sets the client's subscription tier.
    /// </summary>
    public string? Tier { get; set; }
    
    /// <summary>
    /// Gets or sets the client name.
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// Gets or sets whether the client is active.
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Gets or sets additional client attributes.
    /// </summary>
    public Dictionary<string, string> Attributes { get; set; } = new();
}
