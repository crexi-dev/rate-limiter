using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Enums
{
    [Flags]
    public enum ERoleType : int
    {
        DEFAULT = 0,

        /// <summary>
        /// X requests per timespan
        /// </summary>
        XRequestsPerTimespan = 1,

        /// <summary>
        /// a certain timespan passed since the last call
        /// </summary>
        LastCall = 2,

        /// <summary>
        /// for US-based tokens, we use X requests per timespan
        /// </summary>
        USBasedTokens = 4,

        /// <summary>
        /// for EU-based - certain timespan passed since the last call
        /// </summary>
        EUBasedTokens = 8,
    }
}
