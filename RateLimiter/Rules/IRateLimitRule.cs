namespace RateLimiter.Rules;

public interface IRateLimitRule
{
    IRateLimitRule SetNextRule(IRateLimitRule rule);

    bool Handle(Request request);
}