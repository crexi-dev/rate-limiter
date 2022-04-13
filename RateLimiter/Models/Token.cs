using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace RateLimiter.Models
{
    /// <summary>
    /// The Token contains information about the user making the request
    /// </summary>
    public class Token
    {
        /// <summary>
        /// The user name of the user corresponding to this token
        /// </summary>
        public string Subject { get; }

        /// <summary>
        /// A list of claims corresponding to this token
        /// </summary>
        public IReadOnlyDictionary<string, string> Claims { get; }

        public Token(string subject, IDictionary<string, string> claims)
        {
            this.Subject = subject;
            this.Claims = new ReadOnlyDictionary<string, string>(claims);
        }
    }
}
