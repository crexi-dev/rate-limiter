using System;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.Core.Rules
{
    public class TokenBucketRule : IRateLimitRule
    {
        private readonly int _capacity;
        private readonly double _refillRate; // tokens per second
        private readonly ConcurrentDictionary<string, BucketState> _buckets = new();

        private class BucketState
        {
            public double Tokens { get; set; }
            public DateTime LastRefillTime { get; set; }
        }

        public TokenBucketRule(int capacity, double refillRate)
        {
            _capacity = capacity;
            _refillRate = refillRate;
        }

        public bool IsAllowed(string clientToken, string resourceId)
        {
            // create a unique key combining client & resource
            string key = $"{clientToken}:{resourceId}";

            // get or create bucket for this client/resource
            var bucket = _buckets.GetOrAdd(key, val => new BucketState
            {
                Tokens = _capacity,
                LastRefillTime = DateTime.UtcNow
            });

            // refill tokens based on time elapsed
            RefillTokens(bucket);

            // check if we have tokens to consume
            if (bucket.Tokens >= 1)
            {
                bucket.Tokens -= 1;
                return true;
            }

            return false;
        }

        private void RefillTokens(BucketState bucket)
        {
            var now = DateTime.UtcNow;
            var elapsed = (now - bucket.LastRefillTime).TotalSeconds;
            if (elapsed > 0)
            {
                // add tokens based on elapsed time
                var tokensToAdd = elapsed * _refillRate;

                // cap at max capacity
                bucket.Tokens = Math.Min(_capacity, bucket.Tokens + tokensToAdd);
                bucket.LastRefillTime = now;
            }
        }
    }
}