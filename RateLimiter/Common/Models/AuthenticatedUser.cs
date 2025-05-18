namespace RateLimiter.Common.Models;

/// <summary>
/// Represents an authenticated user.
/// </summary>
public class AuthenticatedUser
{
    /// <summary>
    /// Gets or sets the user ID.
    /// </summary>
    public string UserId { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the user's registered region.
    /// </summary>
    public string? Region { get; set; }
    
    /// <summary>
    /// Gets or sets the user's subscription tier.
    /// </summary>
    public string? Tier { get; set; }
    
    /// <summary>
    /// Gets or sets the user's email address.
    /// </summary>
    public string? Email { get; set; }
    
    /// <summary>
    /// Gets or sets additional user claims.
    /// </summary>
    public Dictionary<string, string> Claims { get; set; } = new();
}
