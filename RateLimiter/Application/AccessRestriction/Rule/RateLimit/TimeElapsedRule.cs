using System;
using System.Collections.Generic;
using System.Linq;
using RateLimiter.Application.AccessRestriction.Authorization;
using RateLimiter.Data.Repository;

namespace RateLimiter.Application.AccessRestriction.Rule.RateLimit
{
    ///<inheritdoc cref="ITimeElapsedRule"/>
    public class TimeElapsedRule : TimeRuleBase, ITimeElapsedRule
    {
        private readonly IResourceAccessRepository _resourceAccessRepository;

        ///<inheritdoc/>
        public int MinimumSecondsElapsed { get; set; }

        ///
        public TimeElapsedRule(IResourceAccessRepository resourceAccessRepository)
        {
            _resourceAccessRepository = resourceAccessRepository;
        }

        ///<inheritdoc/>
        protected override IRuleResult ExecuteRule<TResource>(int id, IUserToken userToken)
        {
            RuleResult ruleResult = new(this);

            DateTime lowerBound = DateTime.Now.AddSeconds(-1 * MinimumSecondsElapsed);

            List<IResourceAccess> accesses = _resourceAccessRepository.Get<TResource>(id, userToken.UserId).Where(a => a.Accessed >= lowerBound).ToList();

            if (accesses.Count + 1 > 0)
            {
                ruleResult.IsSuccess = false;
                ruleResult.FailMessage = $"Accessed less than {MinimumSecondsElapsed} seconds ago.";
            }
            else
            {
                ruleResult.IsSuccess = true;
            }

            return ruleResult;
        }
    }
}