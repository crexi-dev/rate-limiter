using System;
using System.Collections.Generic;
using System.Text;

namespace RuleEngine
{
    /// <summary>
    /// RulesEngine class will maintain rules needed to be executed for a resource.
    /// It will be inject in resource constructor. or a RuleEngineFactory class can return ruleEngine instance based on Resource.
    /// </summary>
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
