using System;

namespace RateLimiter.Limits
{
    public class DebounceLimit : LimitBase
    {
        private DateTime? _lastInvokeTime;

        public class DebounceLimitParameters : LimitParametersBase
        {
            public DebounceLimitParameters(TimeSpan timeout)
            {
                Timeout = timeout;
            }

            public TimeSpan Timeout { get; set; }
        }

        public override bool CanInvoke()
        {
            if (_lastInvokeTime != null && DateTime.Now - _lastInvokeTime < (Parameters as DebounceLimitParameters)?.Timeout)
            {
                return false;
            }

            _lastInvokeTime = DateTime.Now;
            return true;
        }
    }
}
