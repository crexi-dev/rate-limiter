using RateLimiter.RateLimiter.Models;
using RateLimiter.RateLimiter.Options;
using System;

namespace RateLimiter.RateLimiter
{
    public class FixedWindowLimiter : ILimiter
    {
        public FixedWindowLimiter(ILimiterOptions options)
        {
            Options = options;
        }

        public ILimiterOptions Options { get; private set; }

        public LimitResult CheckLimit(ClientRequest request)
        {
            var dateDiff = DateTime.UtcNow.Subtract(request.LastHitAt);
            if (dateDiff > Options.Window)
            {
                request.LastHitAt = DateTime.UtcNow;
                request.AmountOfHits = 1;

                return new LimitResult
                {
                    Limited = false,
                    CurrentLimit = Options.Limit,
                    RemainingAmountOfCalls = Options.Limit - request.AmountOfHits,
                };
            }

            if (dateDiff <= Options.Window && request.AmountOfHits >= Options.Limit)
            {
                return new LimitResult
                {
                    Limited = true,
                    CurrentLimit = Options.Limit,
                    RemainingAmountOfCalls = 0,
                    RetryAfterSeconds = (int)(Options.Window - dateDiff).TotalSeconds,
                };
            }

            request.AmountOfHits++;

            return new LimitResult
            {
                Limited = false,
                CurrentLimit = Options.Limit,
                RemainingAmountOfCalls = Options.Limit - request.AmountOfHits,
            };
        }
    }
}
