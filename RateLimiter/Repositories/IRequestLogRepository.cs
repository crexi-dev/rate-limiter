using RateLimiter.Models;
using System;

namespace RateLimiter.Repositories;
public interface IRequestLogRepository
{
    int GetRequestNumber(string resource, DateTime timestamp, Guid clientId, int timeSpanInSeconds);
    void Log(Request token, bool result);
}

public class RequestLogRepository : IRequestLogRepository
{
    public int GetRequestNumber(string resource, DateTime timestamp, Guid clientId,  int timeSpanInSeconds)
    {
        return 10;
    }

    public void Log(Request token, bool result)
    {
        
    }
}
