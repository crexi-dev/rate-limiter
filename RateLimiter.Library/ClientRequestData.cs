using System;

namespace RateLimiter.Library
{
    public struct ClientRequestData
    {
        public int Count;
        public DateTime LastUpdateDate;

        public ClientRequestData(int count, DateTime lastUpdateDate) {
            Count = count;
            LastUpdateDate = lastUpdateDate;
        }
    }
}