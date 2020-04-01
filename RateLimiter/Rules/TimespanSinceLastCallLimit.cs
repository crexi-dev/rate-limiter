using RateLimiter.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Rules
{
    public class TimespanSinceLastCallLimit : IRule
    {
        // per specification: allow request only if a certain timespan passed since the last call per user (token) 
        public bool Validate(IClientRequest request, IRateLimiterManager rate_limiter)
        {
            try
            {
                if (RulesSettings.TimeSpanLimit.Equals(new TimeSpan(0, 0, 0, 0, 0))) // setting is not enabled
                    return true;

                DateTime lastcall = rate_limiter.RequestsLog.Where(l => l.Token.Equals(request.Token)).Max(l => l.CallDateTimeStamp);
                if (lastcall != null &&
                    lastcall > DateTime.Now - RulesSettings.TimeSpanLimit)
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
