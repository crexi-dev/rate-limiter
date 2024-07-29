using System;
using RateLimiter.Models;

namespace RateLimiter.Storage;

public interface IAccessStatisticsRepository
{
    public AccessStatistics Get(string token, Guid resource);
    public void AddAccessRecord(string token, Guid resource, Access record);
}