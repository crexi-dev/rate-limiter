using System.Collections.Generic;

namespace RateLimiter.Rules
{
    public class ChainHelper
    {
        public static IRuleNode OrChain(params IRule<RequestInfo>[] rules)
        {
            List<RuleExecuteNode> rulesNode = new List<RuleExecuteNode>();
            foreach (var item in rules)
            {
                rulesNode.Add(new RuleExecuteNode(item));
            }

            return new OrRule(rulesNode.ToArray());
        }

        public static IRuleNode AndChain(params IRule<RequestInfo>[] rules)
        {
            List<RuleExecuteNode> rulesNode = new List<RuleExecuteNode>();
            foreach (var item in rules)
            {
                rulesNode.Add(new RuleExecuteNode(item));
            }

            return new AndRule(rulesNode.ToArray());
        }
    }
}