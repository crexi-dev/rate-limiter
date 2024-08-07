using System;

namespace RateLimiter.Utilities
{
    /// <summary>
    ///  created a new model to stub for Unit-Testing
    /// </summary>
    public class DateTimeService: IDateTimeService
    {
        public DateTime GetCurrentTime()
        {
            return  DateTime.Now;
        }
    }
}
