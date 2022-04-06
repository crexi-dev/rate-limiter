namespace RateLimiter
{
    public interface ILimitRule
    {
        public abstract string Name { get; }
        public bool Validate(string resource, string identifer);
    }
}
