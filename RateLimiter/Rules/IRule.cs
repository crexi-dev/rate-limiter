namespace RateLimiter.Rules
{
    public interface IRule<T>
    {
        bool Execute(T input);
    }
}