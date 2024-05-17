using System.Collections.Generic;
using RateLimiter.RuleTemplates;
using RateLimiter.RuleTemplates.Params;

namespace RateLimiter
{
    public interface IRuleTemplateDetector
    {
        IList<IRuleTemplate> FindTemplates();
    }
    public class DefaultRuleTemplateDetector : IRuleTemplateDetector
    {
        public IList<IRuleTemplate> FindTemplates()
        {
            return new List<IRuleTemplate>
            {
                new RequestByTimeSpanRuleTemplate(),
                new RegionBasedRuleTemplate(),
                new TimeSpanFromLastCallRuleTemplate()
            };
        }
    }
}