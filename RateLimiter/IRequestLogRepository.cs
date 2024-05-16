using RateLimiter;

public interface IRequestLogRepository
{
    void Log(Request token, bool result);
}