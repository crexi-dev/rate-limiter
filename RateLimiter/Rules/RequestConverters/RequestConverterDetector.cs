using System;
using RateLimiter.Repositories;

namespace RateLimiter.Rules.RequestConverters;

public class RequestConverterDetector : IRequestConverterDetector
{
    public IRequestConverter Construct(Type converterType, IRequestLogRepository requestLogRepository)
    {
        var constructor = converterType.GetConstructor(new[] { typeof(IRequestLogRepository) });
        var instance = constructor.Invoke(new[] { requestLogRepository });
        return (IRequestConverter)instance;
    }
}