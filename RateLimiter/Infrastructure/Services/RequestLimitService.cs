using RateLimiter.Application.Interfaces;
using RateLimiter.Domain.Aggregate;
using RateLimiter.Domain.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Infastructure.Services
{
    public class RequestLimitService : IRequestLimitService, IDisposable
    {
        private readonly ITokenBucket _tokenBucket;
        private readonly IResourceRepository _resourceRepository;
        private readonly ILimitRuleFilter _limitRuleFilter;

        public RequestLimitService(ITokenBucket tokenBucket, IResourceRepository resourceRepository, ILimitRuleFilter limitRuleFilter)
        {
            _tokenBucket = tokenBucket;
            _resourceRepository = resourceRepository;
            _limitRuleFilter = limitRuleFilter;
        }

        /// <summary>
        /// Process shouldn't have cpu bound operation however method can be converted to be async to implement cancellation token.
        /// </summary>
        /// <param name="context"></param>
        public void Evaluate(RequestContext context)
        {
            var resource = _resourceRepository.Get(context.Resource);

            if (resource.HasNoLimitation())
            {
                return; // No limit defined.
            }

            var evaluationContext = new EvaluationContext
            {
                RequestContext = context,
                RuleSet = _limitRuleFilter.FilterByRequest(resource, context)
            };

            _tokenBucket.EvaluateAndUpdate(evaluationContext);
        }

        public void Dispose()
        {
            _tokenBucket.Dispose();
            _resourceRepository.Dispose();
        }
    }
}
