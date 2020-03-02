using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuleEngine
{
    public class RulesEngineSession 
    {
        public RulesEngineSession(IList<IRule> rules)
        {
            _rules = rules;

        }
        public void InsertFact<T>(string name, T fact)
        {
            _facts[name] = fact;
        }


        public void Execute()
        {
            var ruleEnvironment = new RuleEnvironment(_facts);

            foreach (var rule in _rules)
            {
                rule.Execute(ruleEnvironment);
            }
        }


        private class RuleEnvironment : IRuleEnvironment
        {
            public RuleEnvironment(IDictionary<string, object> facts)
            {
                _facts = facts;
            }

            public T GetFact<T>(string name)
            {
                if (_facts.ContainsKey(name))
                    return (T)_facts[name];

                return default(T);
            }

            private IDictionary<string, object> _facts { get; }
        }

        private IList<IRule> _rules;
        private Dictionary<string, object> _facts;
    }
}
