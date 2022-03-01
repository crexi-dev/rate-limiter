using System;
using System.Collections.Generic;

namespace RateLimiter.Tests
{
    public interface IRule
    {
        Func<IJournal, bool> RuleDefinition { get; init; }

        bool Eval(IJournal journal);
    }

    public class Rule : IRule
    {
        public Func<IJournal, bool> RuleDefinition { get; init; }

        public Rule(Func<IJournal, bool> ruleDefinition)
        {
            RuleDefinition = ruleDefinition;
        }

        public bool Eval(IJournal journal) => RuleDefinition.Invoke(journal);
    }
}