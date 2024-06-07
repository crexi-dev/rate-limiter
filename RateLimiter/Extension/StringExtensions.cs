using RateLimiter.Models;
using System;

namespace RateLimiter.Extension;

public static class StringExtensions
{
    public static TimeSpan ParsePeriod(this string period)
    {
        if (string.IsNullOrWhiteSpace(period))
            throw new ArgumentException("Period cannot be null or empty", nameof(period));

        var unit = period[^1];
        var value = int.Parse(period[..^1]);

        return unit switch
        {
            's' => TimeSpan.FromSeconds(value),
            'm' => TimeSpan.FromMinutes(value),
            'h' => TimeSpan.FromHours(value),
            'd' => TimeSpan.FromDays(value),
            _ => throw new ArgumentException($"Invalid period unit '{unit}'", nameof(period))
        };
    }

    public static Region? GetRegionFromToken(this string accessToken)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
            throw new ArgumentException("AccessToken cannot be null or empty", nameof(accessToken));

        if (accessToken.StartsWith("US-"))
            return Region.US;
        if (accessToken.StartsWith("EU-"))
            return Region.EU;
        if (accessToken.StartsWith("RU-"))
            return Region.RU;

        throw new ArgumentException($"Invalid accessToken '{accessToken}'", nameof(accessToken));
    }
}