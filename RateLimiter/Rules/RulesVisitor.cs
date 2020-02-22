using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.Rules
{
    public class RulesVisitor
    {
        private readonly Stack<bool> _stack = new Stack<bool>();
        private RequestInfo _reqInfo;

        public bool Result()
        {
            return _stack.Pop();
        }

        public void AddRequestInfo(RequestInfo reqInfo)
        {
            _reqInfo = reqInfo;
        }

        public void VisitExecute(RuleExecuteNode exe)
        {
            var rslt = exe._rule.Execute(_reqInfo);
            _stack.Push(rslt);
        }

        public void VisitAnd(AndRule and)
        {
            _stack.Push(and._rules.All(l => AcceptAndPop(l) == true));
        }

        public void VisitOr(OrRule or)
        {
            _stack.Push(or._rules.Any(l => AcceptAndPop(l) == true));
        }

        private bool AcceptAndPop(IRuleNode node)
        {
            node.Accept(this);
            return _stack.Pop();
        }
    }
}