using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
    public interface ICacheHelper
    {
        int MaxTimes { get; set; }
        TimeSpan LastRequestTime(string key);
        int RequestsCount(string key, TimeSpan time);
        void AddRequest(string key);
    }
}
