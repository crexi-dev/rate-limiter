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
        private TimeSpan _time_span = new TimeSpan(0, 0, 0, 0, 0); //Days, hours, minutes, seconds, milliseconds

        public TimespanSinceLastCallLimit()
        {
            _time_span = GetTimeSpanFromSettings();
        }

        // per specification: allow request only if a certain timespan passed since the last call
        public bool Validate(IClientRequest request, IRateLimiter rate_limiter)
        {
            if (_time_span.Equals(new TimeSpan(0, 0, 0, 0, 0))) // setting is not enabled
                return true;

            DateTime lastcall = rate_limiter.RequestsLog.Where(l => l.Token.Equals(request.Token)).Max(l => l.CallDateTimeStamp); 
            if (lastcall != null &&
                lastcall > DateTime.Now - _time_span)
                return false;

            return true;
        }

        #region placeholders to grab settings from the DB/FileSystem, etc
        private TimeSpan GetTimeSpanFromSettings()
        {
            return new TimeSpan(1, 2, 0, 30, 0);
        }
        #endregion
    }
}
