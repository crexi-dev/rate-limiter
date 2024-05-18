namespace RateLimiter.Repositories;

public interface IRequestConverterDetector
{
    IRequestConverter Construct(IRequestLogRepository requestLogRepository);
}