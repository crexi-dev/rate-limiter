using System.Collections.Generic;
using RateLimiter.Models;

namespace RateLimiter;

public class ApplicationSettings
{
    public RequestsPerTimespanRuleOptions RequestsPerTimespanRules { get; set; }
    public RequestsPerPeriodRuleOptions RequestsPerPeriodRules { get; set; }
}

public class RequestsPerTimespanRuleOptions : RateLimitOptions
{
    public IEnumerable<RequestsPerTimespanRuleModel> Rules { get; set; }
}

public class RequestsPerPeriodRuleOptions : RateLimitOptions
{
    public IEnumerable<RequestsPerPeriodRuleModel> Rules { get; set; }
}

public class RateLimitOptions
{
    /// <summary>
    /// Enable Rate Limiting
    /// </summary>
    public bool EnableRateLimiting { get; set; } = true;
    /// <summary>
    /// Status code
    /// </summary>
    public int? StatusCode { get; set; } = 429;
    /// <summary>
    /// Message
    /// </summary>
    public string Message { get; set; }
}