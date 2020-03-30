using System;
using RateLimiter.Repository;

namespace RateLimiter.Client
{
    public class RateLimiterProxy : IRateLimiterProxy {
        public bool Verify(string token, DateTime requestDate, string serverIP) {
            var clientRepository = new ClientRepository();
            var rateLimiter = new RateLimiter(clientRepository);

            return rateLimiter.Verify(token, requestDate, serverIP);
        }
    }
}