using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using RateLimiter.API.CounterKeyBuilder;
using RateLimiter.API.Extensions;
using RateLimiter.API.Store;
using RateLimiter.Core.Models.Identity;
using RateLimiter.Core.Models.RateLimit;

namespace RateLimiter.API.Processors;

public class RateLimitProcessor : IRateLimitProcessor
{
    private readonly RateLimitOptions _options;
    private readonly IRateLimitCounterStore _counterStore;
    private readonly IClientCounterKeyBuilder _counterKeyBuilder;

    public RateLimitProcessor(IOptions<RateLimitOptions> options, IClientCounterKeyBuilder counterKeyBuilder,
        IRateLimitCounterStore counterStore)
    {
        _options = options.Value;
        _counterKeyBuilder = counterKeyBuilder;
        _counterStore = counterStore;
    }

    public IEnumerable<RateLimitRule> GetMatchingRules(ClientRequestIdentity identity)
    {
        List<RateLimitRule> limits = new();

        if (_options.LocationRules?.Any() == true)
        {
            LocationRateLimitRule? locationRules =
                _options.LocationRules.FirstOrDefault(x => x != null && x.Location == identity.Location);
            if (locationRules is not null)
            {
                string path = $"{identity.HttpVerb}:{identity.Path}";

                IEnumerable<RateLimitRule> pathLimits =
                    locationRules.Rules.Where(r => path.IsUrlMatch(r.Endpoint));
                limits.AddRange(pathLimits);

                // get the most restrictive limit for each period 
                limits = limits.GroupBy(l => l.Period).Select(l => l.OrderBy(x => x.Limit)).Select(l => l.First())
                    .ToList();
            }
        }

        // search for matching general rules
        if (_options.GeneralRules?.Any() == true)
        {
            List<RateLimitRule> matchingGeneralLimits = new();

            // search for rules with endpoints like "*" and "*:/matching_path" in general rules
            IEnumerable<RateLimitRule> pathLimits = _options.GeneralRules.Where(r =>
                $"*:{identity.Path}".IsUrlMatch(r.Endpoint));
            matchingGeneralLimits.AddRange(pathLimits);

            // search for rules with endpoints like "matching_verb:/matching_path" in general rules
            var verbLimits = _options.GeneralRules.Where(r =>
                $"{identity.HttpVerb}:{identity.Path}".IsUrlMatch(r.Endpoint));
            matchingGeneralLimits.AddRange(verbLimits);

            // get the most restrictive general limit for each period 
            List<RateLimitRule> generalLimits = matchingGeneralLimits
                .GroupBy(l => l.Period)
                .Select(l => l.OrderBy(x => x.Limit).ThenBy(x => x.Endpoint))
                .Select(l => l.First())
                .ToList();

            foreach (RateLimitRule generalLimit in generalLimits.Where(generalLimit =>
                         !limits.Exists(l => l.Period == generalLimit.Period)))
            {
                limits.Add(generalLimit);
            }
        }

        foreach (RateLimitRule item in limits)
        {
            item.PeriodTimespan ??= item.Period.ToTimeSpan();
        }

        limits = limits.OrderBy(l => l.PeriodTimespan).ToList();

        return limits;
    }

    public async Task<RateLimitCounter> ProcessRequestAsync(ClientRequestIdentity requestIdentity, RateLimitRule rule,
        CancellationToken cancellationToken = default)
    {
        RateLimitCounter counter = new()
        {
            Timestamp = DateTime.UtcNow,
            Count = 1
        };

        string counterId = BuildCounterKey(requestIdentity, rule);

        RateLimitCounter? entry = await _counterStore.GetAsync(counterId, cancellationToken);

        if (entry is not null)
        {
            // entry has not expired
            if (rule.PeriodTimespan != null && entry.Timestamp + rule.PeriodTimespan.Value >= DateTime.UtcNow)
            {
                // increment request count
                double totalCount = entry.Count + 1;

                // deep copy
                counter = new RateLimitCounter
                {
                    Timestamp = entry.Timestamp,
                    Count = totalCount
                };
            }
        }

        await _counterStore.SetAsync(counterId, counter, rule.PeriodTimespan!.Value, cancellationToken);

        return counter;
    }

    private string BuildCounterKey(ClientRequestIdentity requestIdentity, RateLimitRule rule)
    {
        string key = _counterKeyBuilder.Build(requestIdentity, rule);

        byte[] bytes = Encoding.UTF8.GetBytes(key);

        using SHA1 algorithm = SHA1.Create();
        byte[] hash = algorithm.ComputeHash(bytes);

        return Convert.ToBase64String(hash);
    }
}