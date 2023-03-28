namespace RateLimiter;

public interface IRequestMatcher
{
    public bool IsMath(RequestInformation request);
}