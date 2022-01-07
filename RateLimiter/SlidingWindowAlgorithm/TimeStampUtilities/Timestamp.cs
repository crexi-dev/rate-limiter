using System.Diagnostics;

namespace RateLimiter.SlidingWindowAlgorithm.TimeStampUtilities
{
    public class Timestamp : ITimestamp
    {
        public long GetTimestamp()
        {
            return Stopwatch.GetTimestamp();
        }
    }
}
