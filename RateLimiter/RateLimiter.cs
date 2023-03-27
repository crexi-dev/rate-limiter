using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RateLimiter.Rules;

namespace RateLimiter
{
    public class RateLimiter
    {
        private ConcurrentDictionary<Guid, Queue<RequestModel>> _clientRequests = new ConcurrentDictionary<Guid, Queue<RequestModel>>();
        private Dictionary<string, List<IRule>> _resourceRules = new Dictionary<string, List<IRule>>();

        public void AddRule(string resource, IRule rule)
        {
            if (!_resourceRules.ContainsKey(resource))
            {
                _resourceRules[resource] = new List<IRule>();
            }

            _resourceRules[resource].Add(rule);
        }

        public bool ProcessRequests(Client client, string resource)
        {
            if (!_resourceRules.ContainsKey(resource))
            {
                AddRequest(client);
                return true;
            }

            if (!_clientRequests.ContainsKey(client.Id))
            {
                AddRequest(client);
                return true;
            }

            foreach (var rule in _resourceRules[resource])
            {
                Queue<RequestModel> newRequestModel = new Queue<RequestModel>(_clientRequests[client.Id]);
                if (!rule.IsAllowed(newRequestModel))
                {
                    return false;
                }
            }

            AddRequest(client);
            return true;
        }

        private void AddRequest(Client client)
        {
            if (!_clientRequests.ContainsKey(client.Id))
            {
                _clientRequests.TryAdd(client.Id, new Queue<RequestModel>());
            }

            _clientRequests[client.Id].Enqueue(new RequestModel(DateTime.UtcNow));
        }
    }
}
