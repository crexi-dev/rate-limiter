using System.Text.RegularExpressions;
using RateLimiter.Core.Models.RateLimit;

namespace RateLimiter.API.Extensions;

public static class Extensions
{
    public static bool IsUrlMatch(this string? source, string value)
    {
        return IsRegexMatch(source, value);
    }

    private static bool IsRegexMatch(this string? source, string value)
    {
        if (source == null || string.IsNullOrEmpty(value))
        {
            return false;
        }

        // if the regex is e.g. /api/values/ the path should be an exact match
        // if all paths below this should be included the regex should be /api/values/*
        if (value[^1] != '$')
        {
            value += '$';
        }

        if (value[0] != '^')
        {
            value = '^' + value;
        }

        return Regex.IsMatch(source, value, RegexOptions.IgnoreCase);
    }

    public static string RetryAfterFrom(this DateTime timestamp, RateLimitRule rule)
    {
        TimeSpan diff = timestamp + rule.PeriodTimespan!.Value - DateTime.UtcNow;
        double seconds = Math.Max(diff.TotalSeconds, 1);

        return $"{seconds:F0}";
    }

    public static TimeSpan ToTimeSpan(this string timeSpan)
    {
        int l = timeSpan.Length - 1;
        string value = timeSpan[..l];
        string type = timeSpan.Substring(l, 1);

        return type switch
        {
            "d" => TimeSpan.FromDays(double.Parse(value)),
            "h" => TimeSpan.FromHours(double.Parse(value)),
            "m" => TimeSpan.FromMinutes(double.Parse(value)),
            "s" => TimeSpan.FromSeconds(double.Parse(value)),
            _ => throw new FormatException($"{timeSpan} can't be converted to TimeSpan, unknown type {type}"),
        };
    }
}