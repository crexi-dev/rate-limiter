using System.Threading.Tasks;

namespace RateLimiter.Algorithms;

public interface IRateLimiterAlgorithm
{
    string AlgorithmName { get; }

    bool Accepted(string api, string token);
}
