using System.Collections.Generic;

namespace RateLimiter.Rules
{
    public class AndRule : IRuleNode
    {
        public List<IRuleNode> _rules;

        public AndRule(params IRuleNode[] ruleNodes)
        {
            _rules = new List<IRuleNode>();

            foreach (var item in ruleNodes)
            {
                if (item is AndRule n)
                    _rules.AddRange(n._rules);
                else
                    _rules.Add(item);
            }
        }

        public void Accept(RulesVisitor v)
        {
            v.VisitAnd(this);
        }
    }
}