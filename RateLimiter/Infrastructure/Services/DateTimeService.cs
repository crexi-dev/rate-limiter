using RateLimiter.Application.Interfaces;
using System;

namespace RateLimiter.Infastructure.Services
{
    public class DateTimeService : IDateTime
    {
        public DateTime Now => DateTime.UtcNow;

        /// <summary>
        /// For simplycity seconds will be the base.
        /// </summary>
        public long Timestamp => (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
    }
}
