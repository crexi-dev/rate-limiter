namespace RateLimiter;

public sealed class RateLimiter
{
    private readonly IRateLimiterStorage _storage;
    private readonly List<RateLimiterRule> _rules;
    private readonly int _limit;
    private readonly int _minLimit;
    private readonly long _maxInterval;
    private readonly List<string> _requiredParameters = new ();

    public RateLimiter(IRateLimiterStorage storage, List<RateLimiterRule> rules)
    {
        if (rules is null)
        {
            throw new ArgumentNullException(nameof(rules));
        }
        
        if (!rules.Any())
        {
            throw new Exception("There must be at least one Rule");
        }
        
        _storage = storage ?? throw new ArgumentNullException(nameof(storage));
        _rules = rules;

        _limit = _rules.Sum(x => x.Limit);
        _minLimit = rules.Min(x => x.Limit);
        _maxInterval = rules.Max(x => x.Interval);

        _requiredParameters = rules.Where(x => x.Parameters is not null && x.Parameters.Count > 0)
            .SelectMany(x => x.Parameters).Select(x => x.Name).Distinct().ToList();
    }

    private int BinarySearch(Attempt[] attempts, int count, long value)
    {
        var left = -1;
        var right = count;

        while (left < right - 1)
        {
            var middle = (left + right) / 2;

            if (attempts[middle].Date < value)
            {
                left = middle;
            }
            else
            {
                right = middle;
            }
        }
        
        return right;
    }

    private ResourceEntry InitialEntry(Attempt attempt)
    {
        var attempts = new Attempt[_limit];

        attempts[0] = attempt;

        return new ResourceEntry(attempts, 1);
    }

    private ResourceEntry RenewEntry(Attempt attempt, ResourceEntry resourceStatistic)
    {
        var attempts = new Attempt[_limit];
        var bound = BinarySearch(resourceStatistic.Attempts, resourceStatistic.Count, attempt.Date - _maxInterval);
        var remainingAttemptsCount = resourceStatistic.Count - bound;

        Array.Copy(resourceStatistic.Attempts, bound, attempts, 0, remainingAttemptsCount);

        return new ResourceEntry(attempts, remainingAttemptsCount);
    }
    
    private void CheckParameters(Attempt attempt)
    {
        if (_requiredParameters.Count == 0)
        {
            return;
        }
        if (attempt.Parameters is not null && _requiredParameters.All(x => attempt.Parameters.Any(y => y.Name == x)))
        {
            return;
        }

        throw new Exception($"There must be required parameters: {string.Join(",", _requiredParameters)}");
    }
    
    public Task<bool> Try(string resourceKey, Attempt attempt)
    {
        CheckParameters(attempt);
        
        var allowed = false;

        _storage.AddOrUpdate(resourceKey,
            _ =>
            {
                //access is allowed because there's no statistic for this resource
                allowed = true;

                return InitialEntry(attempt);
            },
            (_, resourceStatistic) =>
            {
                //access is allowed because the value of the minimal limit less then count
                if (resourceStatistic.Count < _minLimit)
                {
                    allowed = true;

                    return resourceStatistic.WithNewAttempt(attempt);
                }
                
                foreach (var rule in _rules)
                {   
                    if(rule.Parameters is not null && !rule.Parameters.All(p => attempt.Parameters.Contains(p)))
                    {
                        continue;
                    }
                    
                    var intervalBound = attempt.Date - rule.Interval;

                    var indexBound = BinarySearch(resourceStatistic.Attempts, resourceStatistic.Count, intervalBound);

                    var count = resourceStatistic.Count - indexBound;

                    if (count < rule.Limit)
                    {
                        continue;
                    }

                    if (rule.Parameters?.Any() == true)
                    {
                        count = 0;

                        for (var i = indexBound; i < resourceStatistic.Count; i++)
                        {
                            if(rule.Parameters.All(p => resourceStatistic.Attempts[i].Parameters.Contains(p)))
                            {
                                count++;
                            }
                        }

                        if (count < rule.Limit)
                        {
                            continue;
                        }
                    }

                    return RenewEntry(attempt, resourceStatistic);
                }
                
                allowed = true;

                return resourceStatistic.Count >= _limit
                    ? RenewEntry(attempt, resourceStatistic).WithNewAttempt(attempt)
                    : resourceStatistic.WithNewAttempt(attempt);
            });
        
        return Task.FromResult(allowed);
    }
}