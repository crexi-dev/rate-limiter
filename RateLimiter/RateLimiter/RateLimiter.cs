using RateLimiter.Algorithms;
using RateLimiter.Exceptions;
using System.Collections.Generic;

namespace RateLimiter.RateLimiter;

public class RateLimiter : IRateLimiter
{
    private readonly Dictionary<string, IRateLimiterAlgorithm> _algorithms;

    private RateLimiter()
    {
        _algorithms = new Dictionary<string, IRateLimiterAlgorithm>();
    }

    private void CheckAndAddNewAlgorithm(IRateLimiterAlgorithm algorithm)
    {
        if (_algorithms.ContainsKey(algorithm.AlgorithmName))
            throw new LimiterAlgorithmAlreadyExistsException(algorithm.AlgorithmName);
        _algorithms.Add(algorithm.AlgorithmName, algorithm);
    }

    public static IRateLimiter Create(params IRateLimiterAlgorithm[] algorithms)
    {
        var limiter = new RateLimiter();
        if (algorithms == null || algorithms.Length == 0)
            throw new NoLimiterAlgorithmException();

        foreach (var algorithm in algorithms)
            limiter.CheckAndAddNewAlgorithm(algorithm);

        return limiter;
    }

    public void PassOrThrough(string api, string token)
    {
        foreach (var algorithm in _algorithms)
        {
            if (!algorithm.Value.Accepted(api, token))
                throw new LimitExceededException(api, token);
        }
    }
}
