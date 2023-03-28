namespace RateLimiter.Handlers;

public interface IRateLimitHandler
{
    void HandleRequestForUser(RequestInformation request);
}