using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.Interface
{
    public interface IRateLimiterRepository
    {
        void Add<T>(T request);
        IEnumerable<T> RetrieveRequestByBeginningTime<T>(string token, DateTime beginningTime);
        T RetrieverRequestByLastAccessTime<T>(string token);
    }
}
