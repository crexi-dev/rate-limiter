using System.Diagnostics;

namespace RateLimiter.TimeStamp
{
    public class Timestamp : ITimestamp
    {
        public long GetTimestamp()
        {
            return Stopwatch.GetTimestamp();
        }
    }
}
