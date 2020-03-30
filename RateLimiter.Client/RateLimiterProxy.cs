using System;
using RateLimiter.Repository;
using RateLimiter.RulesEngine;

namespace RateLimiter.Client
{
    public class RateLimiterProxy : IRateLimiterProxy {
        public bool Verify(string token, DateTime requestDate, string serverIP) {
            var clientRepository = new ClientRepository();
            var rulesEngineProxy = new RulesEngineProxy();
            var rulesEngineClient = new RulesEngineClient(rulesEngineProxy);
            var rateLimiter = new RateLimiter(clientRepository, rulesEngineClient);

            return rateLimiter.Verify(token, requestDate, serverIP);
        }
    }
}