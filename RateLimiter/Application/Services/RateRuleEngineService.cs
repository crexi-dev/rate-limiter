using System.Collections.Generic;
using System.Diagnostics;
using RateLimiter.Application.AccessRestriction.Authorization;
using RateLimiter.Application.AccessRestriction.Rule;
using RateLimiter.Data.Repository;
using RateLimiter.Domain.Entities;

namespace RateLimiter.Application.Services
{
    /// <inheritdoc />
    public class RateRuleEngineService : IRateRuleEngineService
    {
        private readonly IRuleRepository _ruleRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="RateRuleEngineService"/> class.
        /// </summary>
        /// <param name="ruleRepository">The rule repository.</param>
        public RateRuleEngineService(IRuleRepository ruleRepository)
        {
            _ruleRepository = ruleRepository;
        }

        /// <inheritdoc />
        public IRuleEngineExecutionResult Execute<TResource>(int id, IUserToken userToken)
            where TResource : IResource, new()
        {
            RuleEngineExecutionResult result = new() { RuleResults = new List<IRuleResult>() };

            IRuleSet ruleSet = _ruleRepository.GetAll<TResource>();

            foreach (IRule rule in ruleSet.Rules)
            {
                IRuleResult ruleResult = new RuleResult(rule);

                try
                {
                    ruleResult = rule.Execute<TResource>(id, userToken);

                    if (ruleResult.IsSuccess == false)
                    {
                        Trace.WriteLine("Short circuiting rule execution.");
                        result.RuleResults.Add(ruleResult);
                        break;
                    }
                }
                catch (System.Exception ex)
                {
                    ruleResult.IsSuccess = false;
                    ruleResult.FailMessage = $"Unexpected error, {ex.Message}";
                    result.RuleResults.Add(ruleResult);
                }
            }

            return result;
        }
    }
}