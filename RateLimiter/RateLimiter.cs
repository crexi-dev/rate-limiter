﻿using RateLimiter.Extensions;
using RateLimiter.Model;
using RateLimiter.Repositories;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter
{
    public class RateLimiter
    {
        private static ConcurrentDictionary<string, ConcurrentQueue<DateTime>> _requests = new ConcurrentDictionary<string, ConcurrentQueue<DateTime>>();
        private IResourceRepository _resourceRepository;
        private IClientRepository _clientRepository;

        public RateLimiter(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public bool CanMakeRequest(string userToken, string path)
        {
            var rules = _clientRepository
                        .GetRulesOfResource(userToken.GetClientType(), path)
                        ?
                        .OrderByDescending(x => x.Period)
                        .ToList();

            //If there is no rule request should be proceed
            if (rules == null || !rules.Any())
                return true;

            var now = DateTime.UtcNow;

            if (!_requests.ContainsKey(userToken))
                _requests.TryAdd(userToken, new ConcurrentQueue<DateTime>());

            _requests.TryGetValue(userToken, out var queue);

            foreach (var rule in rules)
            {
                while (queue.TryPeek(out var oldest) && (now - oldest) > rule.Period)
                {
                    queue.TryDequeue(out _);
                }

                if (queue.Count >= rule.Limit)
                {
                    return false;
                }
            }

            queue.Enqueue(now);

            return true;
        }
    }
}
