using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
    public class TimeOfDayLimitRule : RateLimitRule
    {
        private readonly TimeSpan _startTime;
        private readonly TimeSpan _endTime;

        public TimeOfDayLimitRule(TimeSpan startTime, TimeSpan endTime)
        {
            _startTime = startTime;
            _endTime = endTime;
        }

        public override bool AllowRequest(string clientId)
        {
            var now = DateTime.UtcNow.TimeOfDay;
            return now >= _startTime && now <= _endTime;
        }
    }
}
