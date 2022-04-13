using RateLimiter.Rules;

namespace RateLimiter.Interfaces
{
    /// <summary>
    /// This service provides the list of rules for the engine to process. To customize the behavior of the rules engine, 
    /// inject a different service that implements this interface
    /// </summary>
    public interface IRateLimiterRulesEngineOptions
    { 
        public RateLimiterBaseRule[] Rules { get; }
    }
}
