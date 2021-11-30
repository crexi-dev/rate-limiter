using System;
using System.Collections.Generic;
using RateLimiter.Limits;

namespace RateLimiter
{
    public class ParticularResource
    {
        private readonly LimitByClient<DebounceLimit> debounceLimit
            = new LimitByClient<DebounceLimit>(new DebounceLimit.DebounceLimitParameters(TimeSpan.FromSeconds(1)));

        private readonly LimitByClient<ThrottlingLimit> throttlingLimit
            = new LimitByClient<ThrottlingLimit>(new ThrottlingLimit.ThrottlingLimitParameters(2, TimeSpan.FromSeconds(1)));

        public void Fun1(string clientToken)
        {
            if (!debounceLimit.CanInvoke(clientToken))
            {
                return;
            }


            // Go on
        }

        public void Fun2(string clientToken)
        {
            if (!LimitsHelper.CanInvoke(new ILimitByClient[] { debounceLimit, throttlingLimit }, clientToken))
            {
                return;
            }


            // Go on
        }

        public void Fun3(string clientToken)
        {
            var limits = new List<ILimitByClient>();

            if (IsUsaToken(clientToken))
            {
                limits.Add(debounceLimit);
            }
            else
            {
                limits.Add(throttlingLimit);
            }

            if (!LimitsHelper.CanInvoke(limits, clientToken))
            {
                return;
            }


            // Go on
        }

        private bool IsUsaToken(string clientToken)
        {
            return clientToken.Contains("USA");
        }


    }
}
