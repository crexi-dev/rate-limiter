using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Models
{
    public class TokenIntervalModel
    {
        private long _interval;
        private DateTime _lastRequestTime;

        public TokenIntervalModel(long interval)
        {
            _interval = interval;
        }

        public bool allowRequest()
        {
            DateTime now=DateTime.Now;
            if ((now - _lastRequestTime).TotalSeconds < _interval)
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
