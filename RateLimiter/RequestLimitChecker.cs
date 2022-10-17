using System;
using System.Collections.Generic;
using RateLimiter.DataModel;
using RateLimiter.DataAccess.Query;
using RateLimiter.Extensions;
using RateLimiter.Validators;

namespace RateLimiter
{
    public class RequestLimitChecker :  IRequestLimitChecker
    {
        private IQueryRepo _queryRepo;
        private IRuleValidatorFactory _ruleValidatorFactory;

        public RequestLimitChecker(IQueryRepo queryRepo, IRuleValidatorFactory ruleValidatorFactory)
        {
            _queryRepo = queryRepo;
            _ruleValidatorFactory = ruleValidatorFactory;
        }

        public bool CanProcessRequest(RequestData requestData)
        {
            requestData.Mappings = _queryRepo.GetClientResourceRuleMappings();
            var relevantRules = requestData.GetRelevantMappings();
            requestData.RequestedTime = DateTime.Now;
            requestData.HistoryData = HistoryDataUtil.GetHistoryData();

            foreach (var rule in relevantRules)
            {
                var ruleValidator = _ruleValidatorFactory.GetValidator(rule.RuleId);
                if (!ruleValidator.Validate(requestData))
                    return false;
            }

            int count = 0;
            var historyData = requestData.GetRelevantLatestHistoryData();
            if (historyData != null && requestData.RequestedTime.Date == historyData.LastRequested.Date && requestData.RequestedTime.Hour == historyData.LastRequested.Hour && requestData.RequestedTime.Minute == historyData.LastRequested.Minute)
                count = historyData.Count;

            // Below is for History data. This can be added asynchronously too
            requestData.AddHistoryData(new ClientRequestHistory()
            {
                ClientId = requestData.ClientRequest.ClientId,
                ResourceId = requestData.ClientRequest.ResourceId,
                LastRequested = requestData.RequestedTime,
                Count = count + 1,
                CreatedAt = DateTime.Now
            });

            return true;
        }
    }
}
