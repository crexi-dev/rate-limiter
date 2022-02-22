using RateLimits.History;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimits.Rules
{
    public class TimespanSinceLastCallRule : IRule
    {
        private readonly TimeSpan _timeSpan;
        private readonly string _region;

        public TimespanSinceLastCallRule(TimeSpan timeSpan, string region = null)
        {
            _timeSpan = timeSpan;
            _region = region;
        }

        public bool Execute(IEnumerable<HistoryModel> history, string userRegion)
        {
            if (!string.IsNullOrEmpty(_region))
            {
                if (_region != userRegion)
                    return true;
                history = history.Where(x => x.Region == _region);
            }

            var lastCall = history.LastOrDefault();
            if (lastCall == null || DateTime.Now - lastCall.Date > _timeSpan)
                return true;

            return false;
        }
    }
}
