using NUnit.Framework;
using RateLimiter.Rules;

namespace RateLimiter.Tests.Rules
{
    internal class Rule1_True : IRule<RequestInfo>
    {
        public bool Execute(RequestInfo input)
        {
            return true;
        }
    }

    internal class Rule2_False : IRule<RequestInfo>
    {
        public bool Execute(RequestInfo input)
        {
            return false;
        }
    }

    [TestFixture]
    internal class RuleEvalTest
    {
        [Test]
        public void Eval_OneRuleTest()
        {
            var rule = new RuleEval();

            rule.Eval(new Rule2_False());
            var rslt = rule.Evaluate(new RequestInfo());
            Assert.IsFalse(rslt);
        }

        [Test]
        public void OrRule_BothFalse()
        {
            var rule = new RuleEval();

            rule.Eval(new Rule2_False());
            rule.OrRule(new Rule2_False());
            var rslt = rule.Evaluate(new RequestInfo());
            Assert.IsFalse(rslt);
        }

        [Test]
        public void OrRule_TrueAndFalse()
        {
            var rule = new RuleEval();

            rule.Eval(new Rule1_True());
            rule.OrRule(new Rule2_False());
            var rslt = rule.Evaluate(new RequestInfo());
            Assert.IsTrue(rslt);
        }

        [Test]
        public void OrRule_TrueAndTrue()
        {
            var rule = new RuleEval();

            rule.Eval(new Rule1_True());
            rule.OrRule(new Rule1_True());
            var rslt = rule.Evaluate(new RequestInfo());
            Assert.IsTrue(rslt);
        }

        [Test]
        public void AndRule_TrueAndTrue()
        {
            var rule = new RuleEval();

            rule.Eval(new Rule1_True());
            rule.AndRule(new Rule1_True());
            var rslt = rule.Evaluate(new RequestInfo());
            Assert.IsTrue(rslt);
        }

        [Test]
        public void AndRule_TrueAndFalse()
        {
            var rule = new RuleEval();

            rule.Eval(new Rule1_True());
            rule.AndRule(new Rule2_False());
            var rslt = rule.Evaluate(new RequestInfo());
            Assert.IsFalse(rslt);
        }

        [Test]
        public void AndRule_FalseAndFalse()
        {
            var rule = new RuleEval();

            rule.Eval(new Rule2_False());
            rule.AndRule(new Rule2_False());
            var rslt = rule.Evaluate(new RequestInfo());
            Assert.IsFalse(rslt);
        }

        [Test]
        public void Chaining_Or()
        {
            var andChain = ChainHelper.AndChain(new Rule1_True(), new Rule2_False());
            var rule = new RuleEval();

            rule.Eval(new Rule2_False());
            rule.AndChain(andChain);
            var rslt = rule.Evaluate(new RequestInfo());
            Assert.IsFalse(rslt);
        }

        [Test]
        public void Chaining_And()
        {
            var orChain = ChainHelper.OrChain(new Rule1_True(), new Rule2_False());
            var rule = new RuleEval();

            rule.Eval(new Rule2_False());
            rule.OrChain(orChain);
            var rslt = rule.Evaluate(new RequestInfo());
            Assert.IsTrue(rslt);
        }

        [Test]
        public void Complex_Chaining()
        {
            var andChain = ChainHelper.AndChain(new Rule1_True(), new Rule2_False());
            var andChain2 = ChainHelper.AndChain(new Rule1_True(), new Rule1_True());

            var rule = new RuleEval();

            rule.AndChain(andChain);
            rule.OrChain(andChain2);
            var rslt = rule.Evaluate(new RequestInfo());
            Assert.IsTrue(rslt);
        }

        [Test]
        public void Exceptions_Pathing()
        {
            var rule = new RuleEval();

            rule.Eval(new Rule2_False());
            //back to back evals cannot be called.. 
            Assert.Throws<RateLimiterException>(() => rule.Eval(new Rule2_False()));

            var rule2 = new RuleEval();
            // evaluate a rule without inputs
            Assert.Throws<RateLimiterException>(() => rule2.Evaluate(null));

            var rule3 = new RuleEval();
            // evaluate a rule without any rules inputted
            Assert.Throws<RateLimiterException>(() => rule3.Evaluate(new RequestInfo()));

        }
    }
}