using System;
using RateLimiter.Repositories;

namespace RateLimiter.Rules.RequestConverters;

public class RequestConverterFactory : IRequestConverterFactory
{

    private readonly IRequestLogRepository _requestLogRepository;
    private readonly IRequestConverterDetector _requestConverterDetector;

    public RequestConverterFactory(IRequestLogRepository requestLogRepository, IRequestConverterDetector requestConverterDetector)
    {
        _requestLogRepository = requestLogRepository;
        _requestConverterDetector = requestConverterDetector;
    }

    public IRequestConverter Create(RuleValue value)
    {
        var template = value.Template;

        Type converterType = template.GetRequestConverterType();
        var requestConverter = _requestConverterDetector.Construct(converterType, _requestLogRepository);
        return requestConverter;

    }
}
