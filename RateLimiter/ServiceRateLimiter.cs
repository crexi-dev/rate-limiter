using RateLimiter.Data;
using RateLimiter.Helper;
using RateLimiter.Log;
using RateLimiter.Model.Data;
using RateLimiter.Model.Resource;
using RateLimiter.Model.Response;
using RateLimiter.Resource;
using RateLimiter.Rule;
using RateLimiter.Tracking;
using System;
using System.Collections.Generic;

namespace RateLimiter
{

    public class ServiceRateLimiter : IServiceRateLimiter
    {
        private readonly IServiceLogger Logger;
        private readonly IServiceResourceConfigLoader ResourceLoader;
        private readonly IServiceRequestTracker Tracker;
        private readonly IRepositoryRateLimiter Repo;

        public ServiceRateLimiter(IServiceResourceConfigLoader resourceLoader, IRepositoryRateLimiter repo, IServiceLogger logger, IServiceRequestTracker tracker) 
        {
            ResourceLoader = resourceLoader;
            Repo = repo;
            Logger = logger;
            Tracker = tracker;
        }

        public List<ResourceRequestLog> GetRequestLogAll()
        {
            return Repo.GetRequestLogAll();
        }

        public ResponseRateLimitValidation Validate(ResourceRequest request)
        {
            var validationResponse = HelperResponse.GetSuccess<ResponseRateLimitValidation>();            

            try
            {                
                var resource = ResourceLoader.GetConfig(request);

                validationResponse.ResourceConfigId = resource.ConfigId; //for tracking purposes

                ResponseGeneral ruleResponse;

                foreach(var rule in resource.Rules)
                {
                    var validator = ResourceRuleFactory.Get(Repo, rule);
                    ruleResponse = validator.Validate(request);

                    if (!ruleResponse.Success)
                    {
                        validationResponse.ResponseCode = ruleResponse.ResponseCode;
                        validationResponse.ResponseMessage = ruleResponse.ResponseMessage;
                        break;
                    }
                }                
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);

                //TODO: determine how much info to expose here depending how far up the chain this goes...ex you could just make static generic message
                validationResponse = HelperResponse.GetException<ResponseRateLimitValidation>(ex.Message);
            }
            finally 
            {
                Tracker.Track(request, validationResponse);
            }

            return validationResponse;
        }
    }
}
