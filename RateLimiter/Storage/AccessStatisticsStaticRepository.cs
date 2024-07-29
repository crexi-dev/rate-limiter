using System;
using System.Collections.Generic;
using RateLimiter.Models;

namespace RateLimiter.Storage;

public class AccessStatisticsStaticRepository : IAccessStatisticsRepository
{
    private static Dictionary<(string, Guid), AccessStatistics?> AccessStatistics { get; } = new();

    public AccessStatistics Get(string token, Guid resource)
    {
        AccessStatistics.TryGetValue((token, resource), out AccessStatistics? accessStatistics);
        if (accessStatistics == null)
        {
            accessStatistics = new AccessStatistics(token, resource);
            AccessStatistics[(token, resource)] = accessStatistics;
        }

        return accessStatistics;
    }

    public void AddAccessRecord(string token, Guid resource, Access record)
    {
        var accessStatistics = Get(token, resource);
        accessStatistics.AccessList.Add(record);
    }
}