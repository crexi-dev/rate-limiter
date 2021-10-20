using Microsoft.Extensions.Configuration;
using RateLimiter.ConfigurationHelper;
using RateLimiter.Interface;
using RateLimiter.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.Rule.RequestPerTimespan
{
    public class RequestPerTimespanValidator : IRateLimiterVerification
    {
        private readonly TimeSpan _timespanPeriod;
        private readonly int _maxReqeustCountPerTimespan;        

        public RequestPerTimespanValidator(IConfigurationHelper configurationHelper)
        {
            _maxReqeustCountPerTimespan = int.Parse(configurationHelper.MaxRequestCountPerTimespan);
            _timespanPeriod = new TimeSpan(0, 0, 0, int.Parse(configurationHelper.MaxRequestTimespan));
        }

        public bool VerifyAccess(IEnumerable<Request> requests)
        {
            var beginningTimespan = DateTime.Now.Subtract(_timespanPeriod);
            var requestsPerTimespan = requests.Where(x => x.LastAccessTime >= beginningTimespan);
            return _maxReqeustCountPerTimespan >= requestsPerTimespan.Count();
        }
    }    
}
