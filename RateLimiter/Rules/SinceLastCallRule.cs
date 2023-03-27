using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Rules
{
    public class SinceLastCallRule : IRule
    {
        private readonly TimeSpan _minTime;
        public SinceLastCallRule(TimeSpan minTime)
        {
            _minTime = minTime;
        }

        public bool IsAllowed(Queue<RequestModel> requestModelList)
        {
            DateTime lastCall = requestModelList.Peek().RequestDate;
            
            if (DateTime.UtcNow - lastCall < _minTime)
            {
                return false;
            }
            
            return true;
        }
    }
}
