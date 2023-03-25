using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
    public class RateLimiterSettings
    {
        public int Capacity { get; set; }
        public int RefreshRate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity"></param>
        /// <param name="refreshRate">in seconds</param>
        public RateLimiterSettings(int capacity, int refreshRate)
        {
            Capacity = capacity;
            RefreshRate = refreshRate;
        }
    }
}
