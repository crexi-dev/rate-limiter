using System.Collections.Generic;

namespace RateLimiter.Model.Resource.Config
{
    public class ResourceConfig
    {
        public int ConfigId { get; set; }        
        public EnumResource Resource { get; set; }
        public List<Rule.Config.RuleConfigBase> Rules { get; set; } = new List<Rule.Config.RuleConfigBase>();
    }
}
