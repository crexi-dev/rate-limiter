namespace RateLimiter.Rules
{
    public interface IRule
    {
        bool Execute(string token);
    }
}