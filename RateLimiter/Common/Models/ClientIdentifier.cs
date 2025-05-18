namespace RateLimiter.Common.Models;

/// <summary>
/// Represents client information used for rate limiting.
/// </summary>
public class ClientIdentifier
{
    /// <summary>
    /// Gets or sets the client ID.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the client IP address.
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// Gets or sets the client region (e.g., "US", "EU").
    /// </summary>
    public string? Region { get; set; }

    /// <summary>
    /// Gets or sets additional client attributes.
    /// </summary>
    public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
}
