using RateLimiter.Data;
using RateLimiter.Helper;
using RateLimiter.Model.Resource;
using RateLimiter.Model.Response;
using RateLimiter.Model.Rule.Config;
using System;

namespace RateLimiter.Rule.Validator
{
    internal class ValidatorTimespanThreshold : ValidatorBase
    {
        private RuleConfigTimespanThreshold Config;

        public ValidatorTimespanThreshold(IRepositoryRateLimiter repo, RuleConfigTimespanThreshold config) 
            : base(repo, config.Rule)
        {
            Config = config;
        }

        public override ResponseGeneral Validate(ResourceRequest request)
        {
            //we would have this flagged in a db table with country code or something in reality
            var isEUToken = request.Token.ToLower().StartsWith("eu-");

            return isEUToken ? ValidateTimeSinceLastCall(request) : ValidateMaxCallsThreshold(request);
        }

        private ResponseGeneral ValidateMaxCallsThreshold(ResourceRequest request)
        {
            var requests = Repo.GetResourceRequestCount(request.Resource, request.Token, Config.LastCallThreshold);

            return requests < Config.MaxThreshold ?
                HelperResponse.GetSuccess<ResponseGeneral>() :
                HelperResponse.GetFailure<ResponseGeneral>("Resource max requests exceeded.");
        }

        private ResponseGeneral ValidateTimeSinceLastCall(ResourceRequest request)
        {
            var lastRequest = Repo.GetLastResourceRequest(request.Resource, request.Token);
            var lookbackDate = DateTime.Now.Subtract(Config.LastCallThreshold);
            return lastRequest <= lookbackDate ?
                HelperResponse.GetSuccess<ResponseGeneral>() :
                HelperResponse.GetFailure<ResponseGeneral>("Resource recent request exceeded.");
        }
    }    
}
