using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Models
{
    public class TokenInMemoryIntervalModel
    {
        private DateTime _lastRequestTime;

        public TokenInMemoryIntervalModel(DateTime lastRequestTime)
        {
            _lastRequestTime = lastRequestTime;
        }

        public TokenInMemoryIntervalModel()
            : this(DateTime.Now)
        {

        }

        public bool IsAllowed(int period)
        {
            DateTime now = DateTime.Now;
            if ((now - _lastRequestTime).TotalMinutes < period)
            {
                _lastRequestTime = now;
                return false;
            }
            else
            {
                _lastRequestTime = now;
                return true;
            }
        }
    }
}
