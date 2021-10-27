using RateLimiter.Domain.ApiLimiter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Domain.Resource
{
    /// <summary>
    /// Ties a resource to a geo location and its set of rules.
    /// </summary>
    public class ResourceRule
    {
        public string ResourceName { get; set; }
        public string Region { get; set; }
        public IEnumerable<IRule> Rules { get; set; }
    }
}
