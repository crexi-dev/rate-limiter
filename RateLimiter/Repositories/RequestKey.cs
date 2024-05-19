using System;

namespace RateLimiter.Repositories
{
    internal record RequestKey(string Resource, Guid clientId);
    
}