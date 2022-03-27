using Microsoft.Extensions.Caching.Memory;
using RateLimiter.Models;
using RateLimiterMy.Contracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.RateRules
{
    public class RequestsPerTimespanRule : IRule
    {
        private readonly MemoryCache _Cache = new MemoryCache(new MemoryCacheOptions());

        private readonly long _RequestsIntervalLimitTicks;
        private readonly TimeSpan _CashIntervalLimit;
        private readonly int _RequestsLimit;

        public RequestsPerTimespanRule(TimeSpan intervalLimit, int requestsLimit)
        {
            if (requestsLimit <= 0) throw new ArgumentException($"{nameof(intervalLimit)}");

            _RequestsIntervalLimitTicks = intervalLimit.Ticks;
            _RequestsLimit = requestsLimit;

            _CashIntervalLimit = intervalLimit * 2;
        }

        public bool Validate(IRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            long requestTime = (long)(request.Time - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds * TimeSpan.TicksPerMillisecond; 

            var bucket = _Cache.GetOrCreate(request.Token, (item) => { item.SlidingExpiration = _CashIntervalLimit; return new BucketRequestsPerTimespanRule(); });

            return ValidateToken(bucket, requestTime);
        }

        private bool ValidateToken(BucketRequestsPerTimespanRule bucket, long requestTime)
        {
            var requestConforms = false;

            lock (bucket)
            {
                var elapsedTime = bucket.WindowStartTime.HasValue ? (requestTime - bucket.WindowStartTime.Value) : 0;

                if (bucket.WindowStartTime.HasValue)
                {
                    if (elapsedTime >= _RequestsIntervalLimitTicks)
                    {
                        if (elapsedTime >= _RequestsIntervalLimitTicks * 2)
                        {
                            bucket.WindowStartTime = requestTime;
                            bucket.PrevRequestsCount = 0;
                            bucket.RequestsCount = 0;

                            elapsedTime = 0;
                        }
                        else
                        {
                            bucket.WindowStartTime = bucket.WindowStartTime + _RequestsIntervalLimitTicks;
                            bucket.PrevRequestsCount = bucket.RequestsCount;
                            bucket.RequestsCount = 0;

                            elapsedTime = requestTime - bucket.WindowStartTime.Value;
                        }
                    }
                }

                else
                {
                    bucket.WindowStartTime = requestTime;
                }

                var weightedRequestCount = bucket.PrevRequestsCount * ((double)(_RequestsIntervalLimitTicks - elapsedTime) / _RequestsIntervalLimitTicks) + bucket.RequestsCount;//+1
                if (weightedRequestCount < _RequestsLimit)
                {
                    bucket.RequestsCount++;
                    requestConforms = true;
                }
                //else
                //{
                //    double test = weightedRequestCount;
                //}
            }

            return requestConforms;
        }
    }
}
