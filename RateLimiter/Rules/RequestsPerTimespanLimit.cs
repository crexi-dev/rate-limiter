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
        private int _max_requests = -1;
        private TimeSpan _time_span = new TimeSpan(0, 0, 0, 0, 0); //Days, hours, minutes, seconds, milliseconds

        public RequestsPerTimespanLimit()
        {
            _max_requests = GetMaxRequestsFromSettings();
            _time_span = GetTimeSpanFromSettings();
        }

        // per specification, we validate max_requests per user (token) 
        public bool Validate(IClientRequest request, IRateLimiter rate_limiter)
        {
            if (_max_requests == -1)
                return true; // settings are not initiated == no limit. 

            if (rate_limiter.RequestsLog.Count(l => l.Token.Equals(request.Token) &&
                l.CallDateTimeStamp >= DateTime.Now - _time_span) > _max_requests)
                return false;

            return true;
        }

        #region placeholders to grab settings from the DB/FileSystem, etc
        private int GetMaxRequestsFromSettings()
        {
            return 5;
        }

        private TimeSpan GetTimeSpanFromSettings()
        {
            return new TimeSpan(1, 2, 0, 30, 0);
        }
        #endregion
    }
}
