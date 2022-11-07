using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.Interface
{
    /// <summary>
    /// Interface describes general behavior of rule-based request limiter
    /// </summary>
    public interface ILimiter
    {
        /// <summary>
        /// Applies validation routine for specific request token
        /// </summary>
        /// <param name="requestToken"></param>
        /// <returns></returns>
        bool Validate(string requestToken);
    }
}
