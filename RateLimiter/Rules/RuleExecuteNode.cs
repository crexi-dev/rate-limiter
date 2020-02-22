namespace RateLimiter.Rules
{
    public class RuleExecuteNode : IRuleNode
    {
        public IRule<RequestInfo> _rule;

        public RuleExecuteNode(IRule<RequestInfo> rule)
        {
            _rule = rule;
        }

        public void Accept(RulesVisitor v)
        {
            v.VisitExecute(this);
        }
    }
}