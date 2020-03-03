using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Rules
{
    public class Rule10MinPassedSinceLastCall : RuleTimespanPassedSinceLastCall
    {
        private TimeSpan _value = new TimeSpan(0, 10, 0);

        protected override TimeSpan WaitTime { get => _value; }
    }
}
