using RateLimits.History;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimits.Rules
{
    public class RequestsPerTimespanRule : IRule
    {
        private readonly TimeSpan _timeSpan;
        private readonly int _requests;
        private readonly string _region;

        public RequestsPerTimespanRule(TimeSpan timeSpan, int requests, string region = null)
        {
            _timeSpan = timeSpan;
            _requests = requests;
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
            return history.Where(x => x.Date > DateTime.Now - _timeSpan).Count() < _requests;
        }
    }
}
