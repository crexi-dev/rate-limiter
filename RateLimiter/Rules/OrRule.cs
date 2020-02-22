using System.Collections.Generic;

namespace RateLimiter.Rules
{
    public class OrRule : IRuleNode
    {
        public List<IRuleNode> _rules;

        public OrRule(params IRuleNode[] ruleNodes)
        {
            _rules = new List<IRuleNode>();

            foreach (var item in ruleNodes)
            {
                var or = item as OrRule;
                if (or != null)
                    _rules.AddRange(or._rules);
                else
                    _rules.Add(item);
            }
        }

        public void Accept(RulesVisitor v)
        {
            v.VisitOr(this);
        }
    }
}