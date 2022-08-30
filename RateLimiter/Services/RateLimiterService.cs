using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;
using RateLimiter.Exceptions;
using RateLimiter.InMemoryCache.Interfaces;
using RateLimiter.Models;
using RateLimiter.Models.Enums;
using RateLimiter.Models.Rules;
using RateLimiter.Options;
using RateLimiter.Services.Handlers.Models;
using RateLimiter.Services.Interfaces;

namespace RateLimiter.Services
{
    public class RateLimiterService : IRateLimiterService
    {
        private readonly IMediator _mediator;
        private readonly RateLimiterOptions _rateLimiterOptions;
        private readonly IInMemoryCacheProxy _inMemoryCacheProxy;

        public RateLimiterService(IMediator mediator,
            IOptions<RateLimiterOptions> rateLimiterOptions,
            IInMemoryCacheProxy inMemoryCacheProxy)
        {
            _mediator = mediator;
            _inMemoryCacheProxy = inMemoryCacheProxy;
            _rateLimiterOptions = rateLimiterOptions.Value;
        }

        public async Task ValidateRateLimitsAsync(string token, List<RateLimiterType> rateLimiterTypes)
        {
            var existingUserInformation = _inMemoryCacheProxy.GetEntity<UserInformation>(token);
            if (existingUserInformation == null)
            {
                CreateNewUserInformation(token);
                return;
            }

            var isPassed = await ProcessRulesAsync(token, rateLimiterTypes, existingUserInformation);

            existingUserInformation.RequestEntries.Add(DateTime.UtcNow);

            if (!isPassed)
            {
                // should be handled in error handling middleware to set appropriate response status code and response message
                throw new RateLimiterFailedException("Rate Limit validation is not passed.");
            }
        }

        private void CreateNewUserInformation(string token)
        {
            _inMemoryCacheProxy.AddOrUpdateEntity(token, new UserInformation
            {
                Token = token,
                RequestEntries = new List<DateTime> { DateTime.UtcNow }
            });
        }

        private async Task<bool> ProcessRulesAsync(string token, List<RateLimiterType> rateLimiterTypes,
            UserInformation existingUserInformation)
        {
            var isPassed = true;
            foreach (var rateLimiterType in rateLimiterTypes)
            {
                var rateLimiterRule = _rateLimiterOptions.RateLimiterRules[rateLimiterType];
                switch (rateLimiterType)
                {
                    case RateLimiterType.XRequestsPerTimespan:
                        var xRequestsPerTimespanModel = new XRequestsPerTimespanRateLimiterRuleHandlerModel
                        {
                            Token = token,
                            Rule = rateLimiterRule as XRequestsPerTimespanRateLimiterRule,
                            UserInformation = existingUserInformation
                        };
                        isPassed = await _mediator.Send(xRequestsPerTimespanModel);
                        break;
                    case RateLimiterType.CertainTimespanPassedSinceTheLastCall:
                        var certainTimespanPassedSinceTheLastCallRateLimiterRuleModel = new CertainTimespanPassedSinceTheLastCallRateLimiterRuleHandlerModel
                        {
                            Token = token,
                            Rule = rateLimiterRule as CertainTimespanPassedSinceTheLastCallRateLimiterRule,
                            UserInformation = existingUserInformation
                        };
                        isPassed = await _mediator.Send(certainTimespanPassedSinceTheLastCallRateLimiterRuleModel);
                        break;
                }

                // if we need to perform all rules and not until first fail - store bool results of each rule in an array
                if (!isPassed)
                {
                    return false;
                }
            }

            return isPassed;
        }
    }
}
