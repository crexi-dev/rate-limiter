using System;

namespace RateLimiter.Repositories;

public interface IRequestConverterFactory
{
    IRequestConverter Create(RuleValue value);
}

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
        var templateParams = value.Params;

        Type converterType = template.GetRequestConverterType();
        var requestConverter = _requestConverterDetector.Construct(_requestLogRepository);

        return requestConverter;

    }
}