using RateLimiter.RateLimiter.Models;
using RateLimiter.RateLimiter.Options;
using System;

namespace RateLimiter.RateLimiter
{
    public class TimespanLimiter : ILimiter
    {
        public TimespanLimiter(ILimiterOptions options)
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
                    RetryAfterSeconds = (int)Options.Window.TotalSeconds,
                };
            }

            return new LimitResult
            {
                Limited = true,
                CurrentLimit = Options.Limit,
                RemainingAmountOfCalls = 0,
                RetryAfterSeconds = (int)Options.Window.TotalSeconds - (int)dateDiff.TotalSeconds,
            };
        }
    }
}
