namespace RateLimiter.Application.AccessRestriction.Rule
{
    /// <summary>
    /// Association between a resource and a rule governing access to it
    /// </summary>
    public interface IRuleResource
    {
        /// <summary>
        /// Name of the resource
        /// </summary>
        string ResourceName { get; set; }

        /// <summary>
        /// The rule governing access
        /// </summary>
        IRule Rule { get; set; }
    }
}