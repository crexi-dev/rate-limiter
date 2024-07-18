using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter;

public record RateLimiterBuilder<TClient, TResource>(IEnumerable<(Func<(TClient client, TResource resource), bool> predicate, Func<(TClient client, TResource resource), IEnumerable<IRateLimitingRule<TClient, TResource>>> @delegate)> Rules) where TClient : notnull
{
    public RateLimiter<TClient, TResource> Build() => new RateLimiter<TClient, TResource>(new(Rules));

    public RateLimiterBuilderFor<TClient, TResource> For(Func<(TClient client, TResource resource), bool> predicate) => new(this, predicate);
}

public record RateLimiterBuilderFor<TClient, TResource>(RateLimiterBuilder<TClient, TResource> builder, Func<(TClient client, TResource resource), bool> Predicate) where TClient : notnull
{
    public RateLimiterBuilder<TClient, TResource> Apply(Func<(TClient client, TResource resource), IEnumerable<IRateLimitingRule<TClient, TResource>>> @delegate) => builder with { Rules = builder.Rules.Append((Predicate, @delegate)) };
}
