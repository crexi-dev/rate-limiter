using System;
using System.Collections.Concurrent;
using System.Linq;

namespace RateLimiter
{
    public class SlidingWindow
    {
        private double? _windowTime;
        private int? _maxCaunt;
        private int? _timeSinceLastRequest;
        public ConcurrentQueue<long> requests;

        public SlidingWindow(double? windowTime = null, int? maxCaunt = null, int? timeSinceLastRequest = null)
        {
            _windowTime = windowTime != null ? new DateTime().AddMilliseconds((double)windowTime).Ticks : null;
            _maxCaunt = maxCaunt;
            _timeSinceLastRequest = timeSinceLastRequest;
            requests = new ConcurrentQueue<long>();
        }

        public bool AllowRequest()
        {
            var allow = true;
            long curentTime = DateTime.Now.Ticks;

            CheckAndUpdateWindow();

            if (_maxCaunt != null && _windowTime != null) 
            {
                allow = CheckMaxRequestPerTime();
            }

            if (_timeSinceLastRequest != null) 
            {
                allow = CheckTimeSinceLastRequest();
            }

            if (allow) 
            {
                requests.Enqueue(curentTime);
            }

            return allow;
        }

        private bool CheckTimeSinceLastRequest() 
        {
            var lastTimeOfRequest = requests.LastOrDefault();
            return lastTimeOfRequest >= DateTime.Now.AddSeconds(-(double)_timeSinceLastRequest).Ticks;
        }

        private bool CheckMaxRequestPerTime() 
        { 
            if (requests.Count < _maxCaunt)
            {
                return true;
            }
            return false;
        }

        private void CheckAndUpdateWindow() 
        {
            if (requests.IsEmpty) return;

            long peeked = 0;
            requests.TryPeek(out peeked);
            var windowLeaveTime = DateTime.Now.Ticks - peeked;
            while (windowLeaveTime > _windowTime)
            {
                if (requests.TryDequeue(out _) == false || requests.TryPeek(out peeked)) break;
                windowLeaveTime = DateTime.Now.Ticks - peeked;
            }
        }

    }
}
