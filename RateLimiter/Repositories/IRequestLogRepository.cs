using RateLimiter.Models;
using System;

namespace RateLimiter.Repositories;
public interface IRequestLogRepository
{
    int GetRequestNumber(string resource, DateTime timestamp, Guid clientId, int timeSpanInSeconds);
    void Log(Request token, bool result);
}
