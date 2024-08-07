using System;
using System.Collections.Generic;

namespace RateLimiter.Storage
{
    public interface IStorage
    {
        bool AddOrAppend(Guid SessionID);
        bool Exist(Guid SessionID);
        List<DateTime>? Get(Guid SessionID);
        bool Remove(string password, Guid id);
    }
}