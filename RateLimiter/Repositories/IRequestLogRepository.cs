namespace RateLimiter.Repositories;
public interface IRequestLogRepository
{
    void Log(Request token, bool result);
}
