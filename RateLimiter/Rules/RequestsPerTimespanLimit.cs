using RateLimiter.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Rules
{
    public class RequestsPerTimespanLimit : IRule
    {
        public bool Validate(IClientRequest request, IRateLimiterManager rate_limiter)
        {
            try
            {
                if (RulesSettings.MaxRequestsLimit == -1)
                    return true; // settings are not initiated == no limit. 

                if (rate_limiter.RequestsLog.Count(l => l.Token.Equals(request.Token) &&
                    l.CallDateTimeStamp >= DateTime.Now - RulesSettings.TimeSpanLimit) > RulesSettings.MaxRequestsLimit)
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                return false; 
                // Log/Manage exception 
            }
        }
    }
}
