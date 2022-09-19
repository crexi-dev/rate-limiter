using System;

namespace RateLimiter.Rules
{
    public class RequestFrequencyLimiteRule : ILimiteRule
    {
        private object syncObject = new object();

        private DateTime lastRequestTime;
        private long miliseconds;

        public RequestFrequencyLimiteRule(long _miliseconds)
        {
            miliseconds = _miliseconds;
            lastRequestTime = DateTime.Now.AddMilliseconds(-miliseconds);
        }

        public bool CanPassNow(object request)
        {
            lock (syncObject)
            {
                if (lastRequestTime < DateTime.Now.AddMilliseconds(-miliseconds))
                {
                    lastRequestTime = DateTime.Now;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
