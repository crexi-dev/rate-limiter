using System.Net;
using MaxMind.GeoIP2;
using MaxMind.GeoIP2.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RateLimiter.Common.Abstractions;
using RateLimiter.Common.Models;
using RateLimiter.Core.Configuration;

namespace RateLimiter.Infrastructure.Services;

/// <summary>
/// MaxMind GeoIP2 service implementation.
/// </summary>
public class MaxMindGeoIPService : IGeoIPService, IDisposable
{
    private readonly GeoIPOptions _options;
    private readonly ILogger<MaxMindGeoIPService> _logger;
    private readonly DatabaseReader? _databaseReader;

    public MaxMindGeoIPService(
        IOptions<GeoIPOptions> options,
        ILogger<MaxMindGeoIPService> logger)
    {
        _options = options.Value;
        _logger = logger;

        if (!string.IsNullOrEmpty(_options.DatabasePath) && File.Exists(_options.DatabasePath))
        {
            try
            {
                _databaseReader = new DatabaseReader(_options.DatabasePath);
                _logger.LogInformation("MaxMind GeoIP database loaded from {Path}", _options.DatabasePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load MaxMind GeoIP database from {Path}", _options.DatabasePath);
            }
        }
        else
        {
            _logger.LogWarning("MaxMind GeoIP database not configured or file not found at {Path}", _options.DatabasePath);
        }
    }

    /// <summary>
    /// Gets geographic information for an IP address.
    /// </summary>
    public async Task<GeoLocation?> GetLocationAsync(string ipAddress)
    {
        if (_databaseReader == null || string.IsNullOrEmpty(ipAddress))
        {
            return null;
        }

        try
        {
            if (!IPAddress.TryParse(ipAddress, out var ip))
            {
                _logger.LogWarning("Invalid IP address format: {IpAddress}", ipAddress);
                return null;
            }

            // Use Task.Run for the synchronous MaxMind operation
            return await Task.Run(() =>
            {
                try
                {
                    var response = _databaseReader.City(ip);
                    
                    return new GeoLocation
                    {
                        CountryCode = response.Country.IsoCode,
                        CountryName = response.Country.Name,
                        RegionCode = response.MostSpecificSubdivision.IsoCode,
                        RegionName = response.MostSpecificSubdivision.Name,
                        City = response.City.Name,
                        PostalCode = response.Postal.Code,
                        Latitude = response.Location.Latitude,
                        Longitude = response.Location.Longitude,
                        TimeZone = response.Location.TimeZone
                    };
                }
                catch (AddressNotFoundException)
                {
                    _logger.LogDebug("IP address not found in GeoIP database: {IpAddress}", ipAddress);
                    return null;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error looking up IP address in GeoIP database: {IpAddress}", ipAddress);
                    return null;
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during GeoIP lookup for {IpAddress}", ipAddress);
            return null;
        }
    }

    /// <summary>
    /// Gets the business region for an IP address.
    /// </summary>
    public async Task<string> GetRegionAsync(string ipAddress)
    {
        var location = await GetLocationAsync(ipAddress);
        
        if (location?.CountryCode == null)
        {
            return _options.DefaultRegion;
        }

        return MapToBusinessRegion(location.CountryCode);
    }

    private string MapToBusinessRegion(string countryCode)
    {
        // Map country codes to business regions
        return countryCode switch
        {
            "US" or "CA" or "MX" => "NA",
            "GB" or "DE" or "FR" or "IT" or "ES" or "NL" or "BE" or "AT" or "CH" or "SE" or "NO" or "DK" or "FI" => "EU",
            "JP" or "KR" or "SG" or "HK" or "TW" or "AU" or "NZ" => "APAC",
            "BR" or "AR" or "CL" or "PE" or "CO" => "LATAM",
            _ => _options.DefaultRegion
        };
    }

    public void Dispose()
    {
        _databaseReader?.Dispose();
    }
}
