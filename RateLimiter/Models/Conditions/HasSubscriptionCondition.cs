namespace RateLimiter.Models.Conditions
{
    public sealed class HasSubscriptionCondition : ICondition
    {
        private bool hasSubscription;

        public HasSubscriptionCondition(bool hasSubscription)
        {
            this.hasSubscription = hasSubscription;
        }

        public bool IsMatch(IContext context)
        {
            return context.HasSubscription.HasValue && context.HasSubscription.Value == hasSubscription;
        }
    }
}
