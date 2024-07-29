using System;
using System.Collections.Generic;
using System.Linq;
using RateLimiter.Models;
using RateLimiter.Rules;
using RateLimiter.Storage;

namespace RateLimiter;

public class AccessService(Guid resource, IList<IRule> rules, IAccessStatisticsRepository statisticsRepository)
{
    public bool HasAccess(string token)
    {
        var accessStatistics = statisticsRepository.Get(token, resource);
        var result = rules.All(q => q.HasAccess(token, resource, accessStatistics));
        if (result)
        {
            statisticsRepository.AddAccessRecord(token, resource, new Access { AccessTime = DateTimeOffset.Now });
        }
        return result;
    }
}