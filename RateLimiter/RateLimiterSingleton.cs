using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace RateLimiter
{
    public class RateLimiterSingleton
    {
        private static readonly RateLimiterSingleton _instance = new();
        static RateLimiterSingleton() { }
        private RateLimiterSingleton() { }

        private ConcurrentDictionary<Guid, IRateLimiter> _rateLimitersByToken = new();
        public static RateLimiterSingleton Instance
        {
            get
            {
                return _instance;
            }
        }

        public void AddOrReplaceLimiter(Guid token, IRateLimiter rateLimiter)
        {
            if (this._rateLimitersByToken.ContainsKey(token))
            {
                this._rateLimitersByToken[token].Dispose();
            }

            this._rateLimitersByToken[token] = rateLimiter;
        }

        public void RemoveLimiter(Guid token)
        {
            this._rateLimitersByToken.TryRemove(token, out IRateLimiter limiter);
            limiter?.Dispose();
        }

        public async Task<bool> TryCallAsync(Guid token)
        {
            return this._rateLimitersByToken.ContainsKey(token) && await this._rateLimitersByToken[token].TryCallAsync();
        }
    }
}
