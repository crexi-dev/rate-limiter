using RateLimiter.Models;
using RateLimiter.Repositories;
using RateLimiter.Rules;

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
        throw new System.NotImplementedException();
    }
}