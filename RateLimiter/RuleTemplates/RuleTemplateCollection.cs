using System.Collections;
using System.Collections.Generic;

namespace RateLimiter.RuleTemplates;

public class RuleTemplateCollection : List<IRuleTemplate>
{
    public RuleTemplateCollection(IEnumerable<IRuleTemplate> collection) : base(collection)
    {
    }
}