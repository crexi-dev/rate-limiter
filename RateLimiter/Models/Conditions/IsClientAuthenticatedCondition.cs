namespace RateLimiter.Models.Conditions
{
    public sealed class IsClientAuthenticatedCondition : ICondition
    {
        private bool isClientAuthenticated;

        public IsClientAuthenticatedCondition(bool isClientAuthenticated)
        {
            this.isClientAuthenticated = isClientAuthenticated;
        }

        public bool IsMatch(IContext context)
        {
            return context.IsClientAuthenticated.HasValue && context.IsClientAuthenticated.Value == isClientAuthenticated;
        }
    }
}
