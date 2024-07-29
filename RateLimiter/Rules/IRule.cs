using System;
using System.Data;
using RateLimiter.Models;

namespace RateLimiter.Rules;

public interface IRule
{
    bool HasAccess(string token, Guid resource, AccessStatistics statistics);
}