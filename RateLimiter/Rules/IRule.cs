namespace RateLimiter.Rules
{
    internal interface IRule
    {
        public abstract bool IsAllowed(string token);
    }
}
