using RateLimiter.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RateLimiter;

namespace RateLimiter.Resources
{
    public class Resource1 : IResource
    {
        public Resource1(RuleEngine.IRulesEngine rulesEngine, Repository.IRequestThisHour requestThisHour)
        {
            _rulesEngine = rulesEngine;
            _requestThisHour = requestThisHour;
        }
        public void DoWork()
        {
            if (!Validate())
                Console.WriteLine("Failed Validation.");
        }

        public bool Validate()
        {
            var rulesSession = _rulesEngine.NewSession();

            var rulesResult = new RuleResult { Allow = true };
            rulesSession.InsertFact("result", rulesResult);
            rulesSession.InsertFact<int>("requestThisHour", _requestThisHour.GetRequestThisHour());
            rulesSession.Execute();

            return rulesResult.Allow;
        }

        private readonly RuleEngine.IRulesEngine _rulesEngine;
        private readonly Repository.IRequestThisHour _requestThisHour;
    }
}
