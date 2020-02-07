using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Enums
{
    public enum RuleTypes
    {
        /// <summary>
        /// Rules that limit the number of requests per certain period of time.
        /// </summary>
        FrequencyLimiting,

        /// <summary>
        /// Rule that limit the period of time between the requests.
        /// </summary>
        TimeSpanLimiting,

        /// <summary>
        /// Rules that combine both limiting the number of requests and limiting the timespan between the requests.
        /// </summary>
        FrequencyTimeSpanLimiting
    }
}
