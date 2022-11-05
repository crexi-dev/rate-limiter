using Microsoft.Extensions.Caching.Memory;
using RateLimiter.RateLimitRules;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using RateLimiter.Repository;
using RateLimiter.Models;
using System.Linq;

namespace RateLimiter
{
    public class RulesProcessorService : IRulesProcessorService
    {
        private readonly IEventsRepository _eventsRepository;
        private readonly IRulesRepository _rulesRepository;
        public RulesProcessorService(IEventsRepository requestEventsRepository, IRulesRepository rulesRepository)
        {
            _eventsRepository = requestEventsRepository;
            _rulesRepository = rulesRepository;

        }

        public void AddRequestEvent(string key, RequestEvent requestEvent)
        {
            var requestCollection = _eventsRepository.GetById(key);

            requestCollection.Events.Add(requestEvent);

            _eventsRepository.AddOrReplace(requestCollection);

        }


        public bool IsValidLimit(List<string> ruleKeys)
        {
            foreach (var key in ruleKeys)
            {
                var rules = _rulesRepository.GetById(key);

                if (rules.RateLimitRules.Any())
                {
                    foreach (var rule in rules.RateLimitRules)
                    {
                        if (!rule.IsAllowed(_eventsRepository.GetById(key).Events))
                            return false;
                    }
                }

            }

            return true;
        }



    }
}
