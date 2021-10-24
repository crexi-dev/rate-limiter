using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Domain.Resource
{
    /// <summary>
    /// ResourceRules gets serialized into the "Rules" persistence storage
    /// </summary>
    public class ResourceRules
    {
        public List<ResourceRule> ResourceRuleList { get; set; }
    }
}
