using RateLimiter.Models;
using RateLimiter.Repositories;
using RateLimiter.Rules;
using RateLimiter.Rules.Info;
using RateLimiter.RuleTemplates.Params;

namespace RateLimiter.RuleTemplates.RequestConverters;

public class RequestByTimeSpanRuleRequestConverter : IRequestConverter
{
    private readonly IRequestLogRepository _requestLogRepository;

    public RequestByTimeSpanRuleRequestConverter(IRequestLogRepository requestLogRepository)
    {
        _requestLogRepository = requestLogRepository;
    }

    public RuleRequestInfo Convert(Request request, RuleTemplateParams templateParams)
    {
        var currentParameters = templateParams as RequestByTimeSpanRuleTemplateParams;
        if(currentParameters is null)
        {
            throw new InvalidRuleTemplateParamsException();
        }

        var requestNumber = _requestLogRepository
            .GetRequestNumber(request.Resource, request.Timestamp, request.Token.ClientId, currentParameters.TimeSpanInSeconds);
        return new RequestByTimeSpanRuleInfo { Requests = requestNumber };
    }
}