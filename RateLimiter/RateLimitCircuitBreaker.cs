using RateLimiter.Contract;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace RateLimiter
{
    /// <summary>
    /// Very simple implementation of the circuit breaker that allows to decrease number of check when the rate limit exceeded. Optional
    /// </summary>
    internal class RateLimitCircuitBreaker : IRateLimitCircuitBreaker
    {
        // Stored information about what clients where locked (state machine) and at what time
        private readonly ConcurrentDictionary<Guid, DateTime> lockedClients = new ConcurrentDictionary<Guid, DateTime>();
        private readonly RateLimitCircuitBreakerConfiguration rateLimitCircuitBreakerConfiguration;
        private readonly IDateTimeProvider dateTimeProvider;

        public RateLimitCircuitBreaker(
            RateLimitCircuitBreakerConfiguration rateLimitCircuitBreakerConfiguration,
            IDateTimeProvider dateTimeProvider)
        {
            this.rateLimitCircuitBreakerConfiguration = rateLimitCircuitBreakerConfiguration;
            this.dateTimeProvider = dateTimeProvider;
        }

        public bool IsClientLocked(Guid clientId)
        {
            if (this.lockedClients.TryGetValue(clientId, out var clientLockedAt))
            {
                var unlockAt = clientLockedAt + this.rateLimitCircuitBreakerConfiguration.LockFor;

                if (this.dateTimeProvider.UtcNow > unlockAt)
                {
                    this.lockedClients.Remove(clientId, out _);

                    return false;
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        public void LockClient(Guid clientId)
        {
            this.lockedClients[clientId] = this.dateTimeProvider.UtcNow;
        }
    }
}
