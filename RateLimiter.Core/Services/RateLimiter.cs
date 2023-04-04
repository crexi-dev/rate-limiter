using RateLimiter.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RateLimiter.Contracts.Interfaces;

namespace RateLimiter.Core.Services
{
    public class RateLimiter : IRateLimiter
    {
        private readonly int _maxRequests;
        private readonly TimeSpan _timePeriod;
        private readonly List<RateLimitterCounter> _requestTimestamps;

        public RateLimiter(int maxRequests, TimeSpan timePeriod)
        {
            _maxRequests = maxRequests;
            _timePeriod = timePeriod;
            _requestTimestamps = new List<RateLimitterCounter>();
        }

        public bool IsRequestAllowed(string accessToken, string resource)
        {
            _requestTimestamps.RemoveAll(timestamp => timestamp.Resource == resource && timestamp.Token == accessToken && DateTime.UtcNow - timestamp.Date > _timePeriod);

            if (_requestTimestamps.Count(timestamp => timestamp.Resource == resource && timestamp.Token == accessToken) >= _maxRequests)
            {
                return false;
            }

            _requestTimestamps.Add(new RateLimitterCounter { Resource = resource, Date = DateTime.UtcNow, Token = accessToken });
            return true;
        }
    }
}
