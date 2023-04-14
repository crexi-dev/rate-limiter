using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using RateLimiter.Attributes;
using RateLimiter.Attributes.Interfaces;
using RateLimiter.Rules.Interfaces;
using RateLimiter.Stores.Interfaces;

namespace RateLimiter.Rules;

public class LimitPerTimeRule : IRule<LimitPerTimeRateLimitAttribute>
{
    private readonly IRateLimitStore _store;
    public LimitPerTimeRule(IRateLimitStore store)
    {
        _store = store;
    }

    public LimitPerTimeRateLimitAttribute? Parameters { get; set; }
    public async Task<bool> IsRestrict(HttpContext context)
    {
        if (Parameters == null)
        {
            return false;
        }
        
        var range = DateTime.Now.Ticks - Parameters.TimeSpan.Ticks;
        var data = await _store.Get(context.Request.Path);
        var count = 0;

        foreach (var last in data)
        {
            if (last.RequestTicks > range)
            {
                count++;
            }
            else if (last.RequestTicks < range || count >= Parameters.Limit)
            {
                break;
            }
        }
    
        return count >= Parameters.Limit;
    }

    public void SetParameters(IRateLimiterAttribute data)
    {
        Parameters = (data as LimitPerTimeRateLimitAttribute)!;
    }
}