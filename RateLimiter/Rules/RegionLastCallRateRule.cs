using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using RateLimiter.Attributes;
using RateLimiter.Attributes.Interfaces;
using RateLimiter.Rules.Interfaces;

namespace RateLimiter.Rules;

public class RegionLastCallRateRule : IRule<RegionLastCallRateLimitAttribute>
{
    private readonly IRule<LastCallRateLimitAttribute> _lastCallRuleService;
    private readonly IRule<RegionLimitAttribute> _regionRuleService;

    public RegionLastCallRateRule(
        IRule<LastCallRateLimitAttribute> lastCallRuleService, 
        IRule<RegionLimitAttribute> regionRuleService)
    {
        _lastCallRuleService = lastCallRuleService;
        _regionRuleService = regionRuleService;
    }
    
    public RegionLastCallRateLimitAttribute? Parameters { get; set; }
    public async Task<bool> IsRestrict(HttpContext context)
    {
        if (Parameters == null)
        {
            return false;
        }
        
        _regionRuleService.SetParameters(new RegionLimitAttribute(Parameters.Regions));
        _lastCallRuleService.SetParameters(new LastCallRateLimitAttribute(Parameters.TimeSpan));
        
        return await _regionRuleService.IsRestrict(context) && await _lastCallRuleService.IsRestrict(context);
    }

    public void SetParameters(IRateLimiterAttribute data)
    {
        Parameters = (data as RegionLastCallRateLimitAttribute)!;
    }
}