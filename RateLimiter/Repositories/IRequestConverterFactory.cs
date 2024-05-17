namespace RateLimiter.Repositories
{
    public interface IRequestConverterFactory
    {
        IRequestConverter Create(RuleValue value);
    }
}