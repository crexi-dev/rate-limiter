using Microsoft.Extensions.Configuration;
using RateLimiter.ConfigurationHelper;
using RateLimiter.Interface;
using RateLimiter.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.Rule.RequestByLastCall
{
    public class RequestByLastCallValitator : IRateLimiterVerification
    {
        private readonly TimeSpan _lastRequestTimePeriod;       
        public RequestByLastCallValitator(IConfigurationHelper configurationHelper)
        {
            _lastRequestTimePeriod  = new TimeSpan(0, 0, 0, int.Parse(configurationHelper.LastRequestTimePeriod));
        }
        public bool VerifyAccess(IEnumerable<Request> requests)
        {
            var lastAccessTime = requests.OrderBy(x => x.LastAccessTime).Last().LastAccessTime;
            var allowAccessTime = lastAccessTime.Add(_lastRequestTimePeriod);
            return DateTime.Now >= allowAccessTime;
        }
    }
}


