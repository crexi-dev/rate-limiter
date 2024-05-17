using RateLimiter.Models;

namespace RateLimiter.Rules;
public class RegionBasedRule : Rule, IRule
{
    private readonly string _region;
    
    private IRule _innerRule;

    public RegionBasedRule(string region, IRule innerRule)
    {
        _region = region;
        _innerRule = innerRule;
    }

    public override bool Validate(Request request)
    {
        if (request.Token?.Reqion != _region)
        {
            return true;
        }

        return _innerRule.Validate(request);
    }

}
