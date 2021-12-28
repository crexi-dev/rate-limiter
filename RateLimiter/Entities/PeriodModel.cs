using System;

namespace RateLimiter.Entities
{
    public class PeriodModel
    {
        public int PeriodId { get; set; }
        public DateTime? StartPeriod { get; set; }
        public DateTime? EndPeriod { get; set; }
    }
}
