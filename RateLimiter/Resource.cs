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
        ///  if (Key != GetResourceKey(request))
        ///  {
        ///     return RateLimitPartition.CreateNoLimiter(Key);
        ///  }
        ///  if (request.RequestUri.IsLoopback)
        ///  {
        ///    return RateLimitPartition.CreateNoLimiter(Key);
        ///  }
        /// </code>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual RateLimitPartition<string> GetLimiter(HttpRequestMessage request)
        {
            return RateLimitPartition.CreateNoLimiter(Key);
        }

        /// <summary>
        /// Override this to pull the ResourceId from differente place on request Headers, Properties, etc               
        /// <see cref="https://docs.microsoft.com/en-us/dotnet/api/system.uri.absolutepath?view=net-6.0#code-try-1"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual string GetResourceKey(HttpRequestMessage request)
        {
            return request.RequestUri.AbsolutePath;
        }
    }


   
}
