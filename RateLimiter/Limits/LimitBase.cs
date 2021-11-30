namespace RateLimiter.Limits
{
    public abstract class LimitBase
    {
        public abstract class LimitParametersBase
        {
        };

        public LimitParametersBase Parameters { get; set; }

        public abstract bool CanInvoke();
    }
}
