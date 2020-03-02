using System;
using System.Collections.Generic;
using System.Text;

namespace RuleEngine
{
    public class RulesEngine : IRulesEngine
    {
        public RulesEngine()
        {
            _rules = new List<IRule>();
        }

        public void AddRule(IRule rule)
        {
            _rules.Add(rule);
        }

        public RulesEngineSession NewSession()
        {
            return new RulesEngineSession(_rules);
        }

        private List<IRule> _rules;
        private Dictionary<string, object> _globals;
    }
}
