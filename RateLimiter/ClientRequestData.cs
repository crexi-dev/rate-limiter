using System;

namespace RateLimiter
{
    public struct ClientRequestData
    {
        public int Count;
        public DateTime LastUpdateDate;
        public DateTime RequestDate;

        public ClientRequestData(int count, DateTime lastUpdateDate, DateTime requestDate) {
            Count = count;
            LastUpdateDate = lastUpdateDate;
            RequestDate = requestDate;
        }
    }
}