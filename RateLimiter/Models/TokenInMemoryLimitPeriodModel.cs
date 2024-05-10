using System;

namespace RateLimiter.Models
{
    public class TokenInMemoryLimitPeriodModel
    {
        private int _counter;
        private DateTime _lastRequestTime;

        public TokenInMemoryLimitPeriodModel(DateTime lastRequestTime)
        {
            _lastRequestTime = lastRequestTime;
            _counter = 1;
        }

        public TokenInMemoryLimitPeriodModel()
            : this(DateTime.Now)
        {
        }

        public bool IsAllowed(int limit, int period)
        {
            var now = DateTime.Now;

            if ((now - _lastRequestTime).TotalSeconds < period)
            {
                if (_counter >= limit)
                {
                    return false;
                }
                else
                {
                    _counter++;
                    return true;
                }
            }
            else
            {
                _lastRequestTime = now;
                _counter = 1;
                return true;
            }
        }
    }
}
