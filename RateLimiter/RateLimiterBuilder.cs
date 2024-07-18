using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter;

/// <summary>
/// A builder for <see cref="RateLimiter{TClient, TResource}"/> <br/>
/// <example> Use like this:
/// <code>
/// RateLimiterBuilder&lt;string, string&gt; rateLimiterBuilder = new([]);
/// RateLimiter&lt;string, string&gt; rateLimiter =
///                 rateLimiterBuilder
///                 .For(x =&gt; x.client == "1")
///                 .Apply(x =&gt; [new RequestsPerTimeRule&lt;string, string&gt;(timeProvider, 2, TimeSpan.FromMinutes(1))])
///                 .For(x =&gt; x.client == "2")
///                 .Apply(x =&gt; [new RequestsPerTimeRule&lt;string, string&gt;(timeProvider, 3, TimeSpan.FromMinutes(1))])
///                 .Build();
/// </code>
/// </example>
/// </summary>
/// <typeparam name="TClient">The type of client (who is requesting a resource).</typeparam>
/// <typeparam name="TResource">The type of resource (what thing is being accessed).</typeparam>
/// <param name="Rules"></param>
public record RateLimiterBuilder<TClient, TResource>(IEnumerable<(Func<(TClient client, TResource resource), bool> predicate, Func<(TClient client, TResource resource), IEnumerable<IRateLimitingRule<TClient, TResource>>> @delegate)> Rules) where TClient : notnull
{
    /// <summary>
    /// Builds a <see cref="RateLimiter{TClient, TResource}" based on the configured rules./>
    /// </summary>
    /// <returns></returns>
    public RateLimiter<TClient, TResource> Build() => new RateLimiter<TClient, TResource>(new(Rules));

    /// <summary>
    /// Configure for what client / resource combination should rules be built for.
    /// </summary>
    /// <param name="predicate">A predicate to determine the client / resource combinations that a rule will be built for. Kind of like a LINQ where clause. </param>
    /// <returns>Another builder allowing you to specify the rules that will apply when the supplied predicate is met.</returns>
    public RateLimiterBuilderFor<TClient, TResource> For(Func<(TClient client, TResource resource), bool> predicate) => new(this, predicate);
}

/// <summary>
/// See <see cref="RateLimiterBuilder{TClient, TResource}"/>
/// </summary>
/// <typeparam name="TClient"></typeparam>
/// <typeparam name="TResource"></typeparam>
/// <param name="builder"></param>
/// <param name="Predicate"></param>
public record RateLimiterBuilderFor<TClient, TResource>(RateLimiterBuilder<TClient, TResource> builder, Func<(TClient client, TResource resource), bool> Predicate) where TClient : notnull
{
    public RateLimiterBuilder<TClient, TResource> Apply(Func<(TClient client, TResource resource), IEnumerable<IRateLimitingRule<TClient, TResource>>> @delegate) => builder with { Rules = builder.Rules.Append((Predicate, @delegate)) };
}
