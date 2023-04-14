using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using RateLimiter.Attributes;
using RateLimiter.Attributes.Interfaces;
using RateLimiter.Contexts.Interfaces;
using RateLimiter.Rules.Interfaces;

namespace RateLimiter.Rules;

public class RegionRule : IRule<RegionLimitAttribute>
{
    private readonly IUserContext _userContext;
    public RegionRule(IUserContext userContext)
    {
        _userContext = userContext;
    }
    
    public RegionLimitAttribute? Parameters { get; set; }
    public Task<bool> IsRestrict(HttpContext context)
    {
        return Task.FromResult(Parameters != null && Parameters.Regions.Contains(_userContext.Region));
    }

    public void SetParameters(IRateLimiterAttribute data)
    {
        Parameters = (data as RegionLimitAttribute)!;
    }
}