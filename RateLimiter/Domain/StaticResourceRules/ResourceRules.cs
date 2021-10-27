using System.Collections.Generic;

namespace RateLimiter.Domain.Resource
{
    /// <summary>
    /// TODO: ResourceRules gets serialized into the "Rules" persistent storage
    /// </summary>
    public class ResourceRules
    {
        public List<ResourceRule> ResourceRuleList { get; set; }
    }
}
