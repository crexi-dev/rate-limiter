using System;
using System.Collections.Generic;
using System.Linq;
using RateLimiter.Extension;
using RateLimiter.Models;
using RateLimiter.Repositories.Interfaces;
using RateLimiter.Services.Interfaces;

namespace RateLimiter.Services;

public class RequestsPerTimespanValidation(IRateLimitRuleRepository rateLimitRuleRepository) : IRequestValidator
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
        var ruleOptions = RequestsPerTimespanRule();

        if (ruleOptions.EnableRateLimiting == false)
            return (true, 200, string.Empty);

        var relevantRules = RelevantRules(resource, region, ruleOptions);

        if (relevantRules != null)
            foreach (var rule in relevantRules)
            {
                var timespan = rule.Period.ParsePeriod();
                var requestsInTimespan = requestHistory.Values
                    .Count(r => r.Path == resource && r.StatusCode == StatusCodeOk &&
                                r.DateTime > DateTime.UtcNow - timespan);

                if (requestsInTimespan >= rule.Limit)
                {
                    var retryAfter = (requestHistory.Keys.Max() + timespan - DateTime.UtcNow).TotalSeconds;
                    var message = string.Format(ruleOptions?.Message, rule.Limit, timespan, retryAfter);

                    return (false, ruleOptions?.StatusCode, message);
                }
            }

        return _nextHandler?.Check(resource, region, requestHistory) ?? (true, 200, string.Empty);
    }

    private static IEnumerable<RequestsPerTimespanRuleModel> RelevantRules(
        string resource,
        Region? region,
        RequestsPerTimespanRuleOptions ruleOptions)
    {
        return ruleOptions?.Rules?.Where(rule => rule.Endpoint.Equals(resource)
                                                 && (rule.Regions.Length == 0 || rule.Regions.Contains(region.GetValueOrDefault())));
    }

    private RequestsPerTimespanRuleOptions RequestsPerTimespanRule()
    {
        return rateLimitRuleRepository.RequestsPerTimespanRule();
    }
}