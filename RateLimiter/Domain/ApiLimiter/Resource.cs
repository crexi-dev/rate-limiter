using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Domain.ApiLimiter
{
    /// <summary>
    /// For our example, a resource ties to an Api Endpoint.
    /// </summary>
    public class Resource
    {
        public string Name { get; set; }
        public IEnumerable<Rule> ResourceRules { get; set; }
    }
}
