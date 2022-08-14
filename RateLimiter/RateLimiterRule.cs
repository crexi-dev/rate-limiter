using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.RateLimiting;
using System.Threading.Tasks;

namespace RateLimiter
{
    /// <summary>
    /// Extend this to implement the Rule
    /// the system needs check here for 
    /// more information:
    /// <see cref="https://devblogs.microsoft.com/dotnet/announcing-rate-limiting-for-dotnet/#ratelimiter-apis"/>
    /// </summary>
    public abstract class RateLimiterRule
    {
        protected int _retryAfterSeconds;

        public string Id { get; private set; }
        public int RetryAfterSeconds => _retryAfterSeconds;

        public RateLimiterRule(string id)
        {
            _retryAfterSeconds = 0;
            Id = id;
        }

        public abstract bool IsValid<T>(T request);
        public abstract bool IsValid();
    }


    public class RequestInTimeSpan : RateLimiterRule
    {
        public int Requests { get; private set; }
        
        public TimeSpan TimeSpan { get; private set; }

        private readonly TokenBucketRateLimiter _limiter;

        public RequestInTimeSpan(string id, string description, int requests, TimeSpan timeSpan): base(id)
        {
            Requests = requests;
            TimeSpan = timeSpan;            

            _limiter = new TokenBucketRateLimiter(new TokenBucketRateLimiterOptions(tokenLimit: requests, queueProcessingOrder: QueueProcessingOrder.OldestFirst,
                queueLimit: 1, replenishmentPeriod: timeSpan, tokensPerPeriod: 1, autoReplenishment: true));
        }

        public override bool IsValid<HttpRequestMessage>(HttpRequestMessage request)
        {
            return IsValid();
        }

        public override bool IsValid()
        {
            using RateLimitLease lease = _limiter.Acquire(1);
            if (lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
            {
                _retryAfterSeconds = retryAfter.Seconds;
            }
            return lease.IsAcquired;
        }
    }


    public class TimeSpanSinceLast : RateLimiterRule
    {
        public TimeSpan TimeSpan { get; private set; }
        private readonly SlidingWindowRateLimiter _limiter;

        public TimeSpanSinceLast(string id, string description, TimeSpan timeSpan):base(id)
        {
            TimeSpan = timeSpan;
            _limiter = new SlidingWindowRateLimiter(new SlidingWindowRateLimiterOptions(permitLimit: 1,
                queueProcessingOrder: QueueProcessingOrder.OldestFirst, queueLimit: 1, window: timeSpan, segmentsPerWindow: 5, autoReplenishment: true));
        }

        public override bool IsValid<HttpRequestMessage>(HttpRequestMessage request)
        {
            return IsValid();
        }

        public override bool IsValid()
        {
            using RateLimitLease lease = _limiter.Acquire(1);
            if (lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
            {
                _retryAfterSeconds = retryAfter.Seconds;
            }
            return lease.IsAcquired;
        }
    }

    // TODO: Add Rule for US based clients (you can pull the Ip Addres or search for a location on uri ex: us/login) pending on requirements
    // TODO: Add Rule for EU based clients (you can pull the Ip Addres or search for a location on uri ex: eu/login) pending on requirements

}
