namespace RateLimiter.Interfaces
{
    public interface IResouce : IEvaluate
    {
        IResouce AddRule(IRule rule);
    }
}
