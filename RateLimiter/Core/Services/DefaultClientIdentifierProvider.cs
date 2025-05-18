using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using RateLimiter.Common.Abstractions;
using RateLimiter.Common.Models;
using RateLimiter.Core.Configuration;

namespace RateLimiter.Core.Services;

/// <summary>
/// Default implementation for providing client identifiers.
/// </summary>
public class DefaultClientIdentifierProvider : IRateLimitClientIdentifierProvider
{
    private readonly RateLimitOptions _options;

    public DefaultClientIdentifierProvider(IOptions<RateLimitOptions> options)
    {
        _options = options.Value;
    }

    /// <summary>
    /// Gets a client identifier from the HTTP context.
    /// </summary>
    public Task<ClientIdentifier> GetClientIdentifierAsync(HttpContext context)
    {
        var identifier = new ClientIdentifier();
        
        // Try to get client ID from header if specified
        if (!string.IsNullOrEmpty(_options.ClientIdHeaderName) && 
            context.Request.Headers.TryGetValue(_options.ClientIdHeaderName, out var clientIdValues))
        {
            string clientIdValue = clientIdValues.ToString();
            if (!string.IsNullOrEmpty(clientIdValue))
            {
                identifier.Id = clientIdValue;
            }
        }
        
        // If no client ID found, use IP address
        if (string.IsNullOrEmpty(identifier.Id))
        {
            identifier.Id = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }
        
        // Set the IP address
        identifier.IpAddress = context.Connection.RemoteIpAddress?.ToString();
        
        // Try to get region from header if specified
        if (!string.IsNullOrEmpty(_options.RegionHeaderName) && 
            context.Request.Headers.TryGetValue(_options.RegionHeaderName, out var regionValues))
        {
            string regionValue = regionValues.ToString();
            if (!string.IsNullOrEmpty(regionValue))
            {
                identifier.Region = regionValue;
            }
        }
        
        // Add any additional headers as attributes
        foreach (var header in context.Request.Headers)
        {
            if (header.Key.StartsWith("X-") && 
                header.Key != _options.ClientIdHeaderName && 
                header.Key != _options.RegionHeaderName)
            {
                identifier.Attributes[header.Key] = header.Value.ToString();
            }
        }
        
        return Task.FromResult(identifier);
    }
}
