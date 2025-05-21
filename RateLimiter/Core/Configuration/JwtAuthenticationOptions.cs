namespace RateLimiter.Core.Configuration;

/// <summary>
/// Configuration options for JWT authentication.
/// </summary>
public class JwtAuthenticationOptions
{
    /// <summary>
    /// Gets or sets the JWT secret key for validation.
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the expected issuer.
    /// </summary>
    public string? Issuer { get; set; }
    
    /// <summary>
    /// Gets or sets the expected audience.
    /// </summary>
    public string? Audience { get; set; }
    
    /// <summary>
    /// Gets or sets whether JWT authentication is enabled.
    /// </summary>
    public bool Enabled { get; set; } = false;
}
