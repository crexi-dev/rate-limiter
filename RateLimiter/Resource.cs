using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.RateLimiting;
using System.Threading.Tasks;

namespace RateLimiter
{
    /// <summary>
    /// Represent a resource you wanna access 
    /// Rest endpoint, storage, I/O, etc.   
    /// </summary>
    public class Resource
    {
        public string Key { get; private set; }
        public List<RateLimiterRule> Rules { get; set; }

        public Resource(string key, List<RateLimiterRule> rules)
        {
            Key = key;
            Rules = rules;
        }
    }
}
