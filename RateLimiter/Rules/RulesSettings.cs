using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Rules
{
    public static class RulesSettings
    {
        public static TimeSpan TimeSpanLimit = new TimeSpan(0, 0, 0, 5, 0); //Days, hours, minutes, seconds, milliseconds
        public const int MaxRequestsLimit = 5; 
    }
}
