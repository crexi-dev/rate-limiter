public interface IRateLimiterService
{
    bool Validate(string resource, string token);
}