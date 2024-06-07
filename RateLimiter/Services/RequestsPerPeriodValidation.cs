using System;
using System.Collections.Generic;
using System.Linq;
using RateLimiter.Extension;
using RateLimiter.Models;
using RateLimiter.Repositories.Interfaces;
using RateLimiter.Services.Interfaces;

namespace RateLimiter.Services;

public class RequestsPerPeriodValidation(IRateLimitRuleRepository rateLimitRuleRepository) : IRequestValidator
{
    private IRequestValidator _nextHandler;
    private const int StatusCodeOk = 200;

    public IRequestValidator SetNext(IRequestValidator next)
    {
        _nextHandler = next;
        return _nextHandler;
    }

    public (bool isAllowed, int? statusCode, string message) Check(string resource, Region? region, IDictionary<DateTime, RateLimitRequestModel> requestHistory)
    {
        var ruleOptions = TimespanSinceLastRequestRule();

        if (ruleOptions.EnableRateLimiting == false)
            return (true, 200, string.Empty);

        var relevantRules = RelevantRules(resource, region, ruleOptions);

        if (relevantRules != null)
            foreach (var rule in relevantRules)
            {
                var timespan = rule.Period.ParsePeriod();
                var lastRequestTime = requestHistory.Values
                    .Where(r => r.Path == resource && r.StatusCode == StatusCodeOk)
                    .OrderByDescending(r => r.DateTime)
                    .Select(r => r.DateTime)
                    .FirstOrDefault();

                if (lastRequestTime != default && (DateTime.UtcNow - lastRequestTime) < timespan)
                {
                    var retryAfter = (lastRequestTime + timespan - DateTime.UtcNow).TotalSeconds;
                    var message = string.Format(ruleOptions?.Message, timespan, retryAfter);

                    return (false, ruleOptions?.StatusCode, message);
                }
            }

        return _nextHandler?.Check(resource, region, requestHistory) ?? (true, 200, string.Empty);
    }

    private static IEnumerable<RequestsPerPeriodRuleModel> RelevantRules(string resource, Region? region, RequestsPerPeriodRuleOptions ruleOptions)
    {
        return ruleOptions?.Rules?.Where(rule => rule.Endpoint.Equals(resource) && (rule.Regions.Length == 0 || rule.Regions.Contains(region.GetValueOrDefault())));
    }

    private RequestsPerPeriodRuleOptions TimespanSinceLastRequestRule()
    {
        return rateLimitRuleRepository.RequestsPerPeriodRule();
    }
}