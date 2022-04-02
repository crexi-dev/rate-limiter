using System.Collections.Generic;

namespace RateLimiter.Models
{
    public class RequestHistory
    {
        public List<RateLimiting> History { get; set; }
        public RequestHistory(List<RateLimiting> history)
        {
            History = history;
        }
    }
}
