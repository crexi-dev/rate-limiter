using RateLimiter.Interface;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RateLimiter
{
    /// <summary>
    /// Request limiter based on number of requests per timespan
    /// </summary>
    public class FrequenceLimiter : ILimiter
    {
        private readonly TimeSpan _timeSpan;
        private readonly uint _frequence;
        private ConcurrentDictionary<string, LinkedList<DateTime>> _histories;

        public FrequenceLimiter(uint frequence, TimeSpan timeSpan)
        {
            _frequence = frequence;
            _timeSpan = timeSpan;
            _histories = new ConcurrentDictionary<string, LinkedList<DateTime>>();
        }

        public bool Validate(string requestToken)
        {
            // frequence 0 means no limitation
            if (_frequence == 0)
            {
                return true;
            }

            var history = _histories.GetOrAdd(requestToken, new LinkedList<DateTime>());

            history.AddFirst(DateTime.UtcNow);

            // check if history is ok.
            var now = DateTime.UtcNow;
            var spanStart = now - _timeSpan;

            // remove all expired requests from history
            for(var last = history.Last; last.Value < spanStart; last = history.Last)
            {
                history.Remove(last);
            }

            return history.Count <= _frequence;
        }
    }
}
