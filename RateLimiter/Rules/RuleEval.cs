using System.Collections.Generic;

namespace RateLimiter.Rules
{
    public class RuleEval
    {
        private readonly RulesVisitor _rulesVisitor;
        private readonly Stack<IRuleNode> _lastRule;
        private IRuleNode _ruleNodeCache;

        public RuleEval()
        {
            _rulesVisitor = new RulesVisitor();
            _lastRule = new Stack<IRuleNode>();
        }

        public bool Evaluate(RequestInfo reqInfo)
        {
            if (reqInfo == null)
            {
                throw new RateLimiterException("Missing request input");
            }
            if (_lastRule.Count == 0 && _ruleNodeCache == null)
            {
                throw new RateLimiterException("Missing entries for a rule. Please call First, Or, Add functions to add a new rule");
            }

            if (_lastRule.Count > 0)
            {
                _ruleNodeCache = _lastRule.Pop();
            }

            _rulesVisitor.AddRequestInfo(reqInfo);

            _ruleNodeCache.Accept(_rulesVisitor);
            return _rulesVisitor.Result();
        }

        public void Eval(IRule<RequestInfo> rule)
        {
            if (_lastRule.Count == 1)
            {
                throw new RateLimiterException("Eval rule can only be invoked once. Please add a rule using Or rule or And rule");
            }
            _lastRule.Push(new RuleExecuteNode(rule));
        }

        public void OrRule(IRule<RequestInfo> rule)
        {
            if (_lastRule.Count == 0)
            {
                _lastRule.Push(new OrRule(new RuleExecuteNode(rule)));
            }

            var leftSide = _lastRule.Pop();
            _lastRule.Push(new OrRule(leftSide, new RuleExecuteNode(rule)));
        }

        public void AndRule(IRule<RequestInfo> rule)
        {
            if (_lastRule.Count == 0)
            {
                _lastRule.Push(new AndRule(new RuleExecuteNode(rule)));
            }
            var leftSide = _lastRule.Pop();
            _lastRule.Push(new AndRule(leftSide, new RuleExecuteNode(rule)));
        }

        public void AndChain(IRuleNode andNode)
        {
            if (_lastRule.Count == 0)
            {
                _lastRule.Push(new AndRule(andNode));
            }
            var leftSide = _lastRule.Pop();
            _lastRule.Push(new AndRule(leftSide, andNode));
        }

        public void OrChain(IRuleNode orNode)
        {
            if (_lastRule.Count == 0)
            {
                _lastRule.Push(new OrRule(orNode));
            }
            var leftSide = _lastRule.Pop();
            _lastRule.Push(new OrRule(leftSide, orNode));
        }
    }
}