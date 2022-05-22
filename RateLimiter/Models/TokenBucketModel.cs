using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Models
{
    public class TokenBucketModel
    {
        private long _maxBucketSize;
        private long _refillRate;

        private double _currentBucketSize;
        private DateTime _lastRefillTime;

        public TokenBucketModel(long maxBucketSize, long refillRate)
        {
            _maxBucketSize = maxBucketSize;
            _refillRate = refillRate;

            _currentBucketSize = maxBucketSize;
            _lastRefillTime = DateTime.Now;
        }

        public bool allowRequest(int tokens=1)
        {
            refill();
            if (_currentBucketSize >= tokens)
            {
                _currentBucketSize -= tokens;
                return true;
            }

            return false;
        }

        public void  refill()
        {
            DateTime currTime = DateTime.Now;
            double tokensToAdd = (currTime - _lastRefillTime).TotalSeconds * _maxBucketSize / _refillRate;
            _currentBucketSize = Math.Min(_currentBucketSize + tokensToAdd, _maxBucketSize);
            _lastRefillTime = DateTime.Now;
        }
    }
}
