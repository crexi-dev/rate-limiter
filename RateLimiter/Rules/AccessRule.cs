namespace RateLimiter.Rules
{
    public abstract class AccessRule
    {
        public abstract bool Validate(string resourceName, string accessKey);
    }
}
