using System;
using RateLimiter.Repositories;

namespace RateLimiter.Rules.RequestConverters;

public interface IRequestConverterDetector
{
    IRequestConverter Construct(Type converterType, IRequestLogRepository requestLogRepository);
}