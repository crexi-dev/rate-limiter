using System;
using System.Collections.Generic;

namespace RateLimiter.Limits
{
    public class ThrottlingLimit : LimitBase
    {
        public class ThrottlingLimitParameters : LimitParametersBase
        {
            public ThrottlingLimitParameters(int allowedCallsPerTimespan, TimeSpan timespan)
            {
                if (allowedCallsPerTimespan < 0)
                {
                    throw new ArgumentException("Number of calls less than 0 not allowed");
                }

                AllowedCallsPerTimespan = allowedCallsPerTimespan;
                Timespan = timespan;
            }

            public int AllowedCallsPerTimespan { get; set; }
            public TimeSpan Timespan { get; set; }
        }

        private Queue<DateTime> _callQueue = new Queue<DateTime>();

        public override bool CanInvoke()
        {
            var allowedCallsPerTimespan = (Parameters as ThrottlingLimitParameters)?.AllowedCallsPerTimespan;
            var timespan = (Parameters as ThrottlingLimitParameters)?.Timespan;

            while (true)
            {
                if (_callQueue.Count == 0)
                {
                    break;
                }

                var callTime = _callQueue.Peek();

                if (DateTime.Now - callTime <= timespan)
                {
                    break;
                }

                _callQueue.Dequeue();
            }

            _callQueue.Enqueue(DateTime.Now);

            return _callQueue.Count <= allowedCallsPerTimespan;
        }
    }
}
