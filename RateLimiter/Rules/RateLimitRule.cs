using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Linq;

namespace RateLimiter.Rules;

public abstract class RateLimitRule<T> : IRateLimitRule where T : IRateLimitRuleOptions
{
    protected ILogger Logger { get; }

    protected T Options { get; }

    protected RateLimitRule(T options) : this(options, null) { }
    protected RateLimitRule(T options, ILogger? logger = null)
    {
        Options = options;
        Logger = logger ?? NullLoggerFactory.Instance.CreateLogger<RateLimitRule<T>>();
    }

    public abstract bool IsRequestAllowed(AccessToken token, string route);

    protected bool ValidateCondition(AccessToken token)
    {
        //apply rule based on condition or for all requests if condition is not set
        return Options.RuleConditions == null || Options.RuleConditions.All(condition => condition(token));
    }
}
