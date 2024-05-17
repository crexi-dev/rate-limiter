using System.Collections;
using System.Collections.Generic;
using RateLimiter.RuleTemplates;

namespace RateLimiter;

public class RuleTemplateCollection : List<IRuleTemplate>
{
    public RuleTemplateCollection(IEnumerable<IRuleTemplate> collection) : base(collection)
    {
    }
}