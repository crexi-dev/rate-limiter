namespace RateLimiter.Models
{
    public class LimitPeriodModel
    {
        public int? HoursLimit { get; set; }
        public int? DaysLimit { get; set; }
        public int? MinutesLimit { get; set; }
    }
}
