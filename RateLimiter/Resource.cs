using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.RateLimiting;

namespace RateLimiter
{
    /// <summary>
    /// Default Resource implementation, Inherit from this clase
    /// for more complex combination and rules
    /// <seealso cref="https://devblogs.microsoft.com/dotnet/announcing-rate-limiting-for-dotnet/#ratelimiter-apis"/>
    /// </summary>
    public class Resource
    {
        public string Key { get; private set; }

        public Resource(string key)
        {
            Key = key;
        }

        /// <summary>
        /// Default to Not Limit, override to add your custom rules    
        /// <code>        
        ///  if (request.RequestUri.IsLoopback)
        ///  {
        ///    return RateLimitPartition.CreateNoLimiter(Key);
        ///  }
        /// </code>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual PartitionedRateLimiter<HttpRequestMessage> GetLimiter(HttpRequestMessage request)
        {
            return PartitionedRateLimiter.Create<HttpRequestMessage, string>(resource =>
            {
                return RateLimitPartition.CreateNoLimiter(Key);
            });
        }
        
    }


   
}
