namespace RateLimiter.Rules;

public abstract class RateLimitRule : IRateLimitRule
{
    private IRateLimitRule? _nextRule;

    public IRateLimitRule SetNextRule(IRateLimitRule rule)
    {
        _nextRule = rule;

        return rule;
    }

    public virtual bool Handle(Request request)
    {
        return _nextRule?.Handle(request) ?? true;
    }
}