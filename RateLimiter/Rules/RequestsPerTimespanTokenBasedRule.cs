using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using RateLimiter.Models;

namespace RateLimiter.Rules;

public class RequestsPerTimespanTokenBasedRule : RequestsPerTimespanRule
{
    private readonly Regex _regex;
    
    public RequestsPerTimespanTokenBasedRule(TimeSpan requestTimeSpan, int requestCount, string tokenPattern) 
        : base(requestTimeSpan, requestCount)
    {
        _regex = new Regex(tokenPattern, RegexOptions.Compiled);
    }

    public override async Task<bool> CheckAsync(RequestData obj, CancellationToken cancellationToken)
    {
        if (!_regex.Match(obj.Token).Success)
        {
            return await Task.FromResult(true);
        }
        return await base.CheckAsync(obj, cancellationToken).ConfigureAwait(false);
    }
    
    public new string Description => $"Rule {AllowedRequestCount} requests per {RequestTimeSpan} for {_regex}";
}