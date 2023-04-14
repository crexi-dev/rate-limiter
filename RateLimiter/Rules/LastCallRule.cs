using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using RateLimiter.Attributes;
using RateLimiter.Attributes.Interfaces;
using RateLimiter.Rules.Interfaces;
using RateLimiter.Stores.Interfaces;

namespace RateLimiter.Rules;

public class LastCallRule : IRule<LastCallRateLimitAttribute>
{
    private readonly IRateLimitStore _store;

    public LastCallRule(IRateLimitStore store)
    {
        _store = store;
    }

    public LastCallRateLimitAttribute? Parameters { get; set; }
    
    public async Task<bool> IsRestrict(HttpContext context)
    {
        if (Parameters == null)
        {
            return false;
        }
        
        var last = await _store.GetLast(context.Request.Path);
        return last != null && last.RequestTicks > DateTime.Now.Ticks - Parameters.TimeDelta.Ticks;
    }

    public void SetParameters(IRateLimiterAttribute data)
    {
        Parameters = (data as LastCallRateLimitAttribute)!;
    }
}