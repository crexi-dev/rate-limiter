using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RateLimiter.Model;
using RateLimiter.Repository;
using RuleEngine;

namespace RateLimiter.Resources
{
    public abstract class ResourceBase : IResource
    {
        protected readonly string _token;
        protected readonly IRulesEngine _rulesEngine;
        protected readonly IRetrieveTokenInfo _retrieveTokenInfo;

        public ResourceBase(string token, RuleEngine.IRulesEngine rulesEngine, IRetrieveTokenInfo retrieveTokenInfo)
        {
            _token = token;
            _rulesEngine = rulesEngine;
            _retrieveTokenInfo = retrieveTokenInfo;
        }
        public bool CanContinue()
        {

            var rulesSession = _rulesEngine.NewSession();

            var rulesResult = new RuleResult { Allow = true };
            rulesSession.InsertFact("result", rulesResult);
            rulesSession.InsertFact<TokenInfo>("tokenInfo", _retrieveTokenInfo.GetTokenInfo(_token));
            rulesSession.Execute();

            return rulesResult.Allow;
        }

        public abstract void DoWork();
    }
}
