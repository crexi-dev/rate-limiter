using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RateLimiter.Configuration;

namespace RateLimiter.Rules
{
    public class TimeFromLastCallRule : IRule
    {
        /// <summary>
        /// Minimum time between two consecutive checks
        /// </summary>
        public TimeSpan TimeSpan { get; set; }

        public bool Check(ICollection<DateTime> accessAttempts)
        {
            if (accessAttempts.Count < 2)
            {
                return true;
            }

            var lastAttempts = accessAttempts.TakeLast(2).ToArray();

            return lastAttempts[1] - lastAttempts[0] > TimeSpan;
        }
    }
}
