using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using RateLimiter.Common.Abstractions.Rules;
using RateLimiter.Common.Models;
using RateLimiter.Core.Services.KeyBuilders;

namespace RateLimiter.Core.Services;

/// <summary>
/// Builds rate limiting keys based on resources.
/// </summary>
public class ResourceKeyBuilder : IKeyBuilder
{
    private readonly bool _includeHttpMethod;
    private readonly bool _normalizeResourceNames;

    public ResourceKeyBuilder(bool includeHttpMethod = true, bool normalizeResourceNames = true)
    {
        _includeHttpMethod = includeHttpMethod;
        _normalizeResourceNames = normalizeResourceNames;
    }

    /// <summary>
    /// Builds a key for rate limiting.
    /// </summary>
    public string BuildKey(HttpContext context, IRateLimitRule rule, ClientIdentifier clientIdentifier)
    {
        var keyParts = new List<string> { rule.Name, clientIdentifier.Id };
        
        // Add HTTP method if configured
        if (_includeHttpMethod)
        {
            keyParts.Add(context.Request.Method);
        }
        
        // Add the resource path
        string resourcePath = context.Request.Path.Value?.TrimStart('/') ?? string.Empty;
        
        // Normalize resource path if configured
        if (_normalizeResourceNames && !string.IsNullOrEmpty(resourcePath))
        {
            resourcePath = NormalizeResourcePath(resourcePath);
        }
        
        keyParts.Add(resourcePath);
        
        // Add region if available
        if (!string.IsNullOrEmpty(clientIdentifier.Region))
        {
            keyParts.Add(clientIdentifier.Region);
        }
        
        // Combine all parts with a separator
        return string.Join(":", keyParts);
    }
    
    /// <summary>
    /// Normalizes a resource path by replacing numeric IDs with {id} placeholders.
    /// </summary>
    private string NormalizeResourcePath(string path)
    {
        // Replace numeric path segments with {id}
        return Regex.Replace(path, "/\\d+(/|$)", "/{id}$1");
    }
}
