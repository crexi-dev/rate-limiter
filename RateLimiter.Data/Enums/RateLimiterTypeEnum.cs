using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Data.Enums
{
    [Flags]
    public enum RateLimiterTypeEnum
    {

        RequestPerTimespan = 1,
        RequestDebounce = 2,

        AutoByRegion = 4
    }
}
