using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace RateLimiter.Interface
{
    /// <summary>
    /// Generic interface for parsing of request token to get related properties
    /// </summary>
    public interface ITokenParser
    {
        /// <summary>
        /// Converts if possibe original token string into set of named text properties
        /// </summary>
        /// <param name="requestToken"></param>
        /// <returns></returns>
        NameValueCollection  Parse(string requestToken);
    }
}
