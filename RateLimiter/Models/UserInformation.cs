using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace RateLimiter.Models
{
    /// <summary>
    /// Super easy/light representation of User and auth Token - as real implementation of rate limit
    /// logic is not a part of the task as it's written in the description 
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class UserInformation
    {
        public string Token { get; set; }
        // As our algorithms are very simple - just store all request entries of the user
        public List<DateTime> RequestEntries { get; set; }

        public UserInformation()
        {
            RequestEntries = new List<DateTime>();
        }
    }
}
