namespace RateLimiter.Models.Conditions
{
    public sealed class RegionCondition : ICondition
    {
        private string region;

        public RegionCondition(string region)
        {
            this.region = region;
        }

        public bool IsMatch(IContext context)
        {
            return context.Region != null && context.Region == region;
        }
    }
}
