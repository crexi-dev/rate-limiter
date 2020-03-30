using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using RateLimiter.Implementation;
using RateLimiter.RulesEngine;
using RateLimiter.Repository;

namespace RateLimiter
{
    public class RateLimiter : IRateLimiter
    {
        private IClientRepository clientRepository;
        private IRulesEngineClient rulesEngineClient;

        public RateLimiter(IClientRepository clientRepository, IRulesEngineClient rulesEngineClient) {
            this.clientRepository = clientRepository;
            this.rulesEngineClient = rulesEngineClient;
        }

        public bool Verify(string token, DateTime requestDate, string serverIP) {
            // get rule based on client request data
            var rules = this.rulesEngineClient.GetRules(serverIP);

            // get rate limit settings

            // retrieve client data
            var clientData = this.clientRepository.GetClientData(token);

            // verify w/in rate limits (invoke appropriate rate limiter methods)

            var isAllowed = false;

            if (!isAllowed)
                return false;
            
            else {
                // update client data

                return true;
            }
        }
    }
}