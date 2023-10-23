using RateLimiter.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.Data
{
    public partial class DummyRepository : IRepository
    {
        private HashSet<IRequestLimitRule> _rules = new();
        private Dictionary<Guid, IClient> _clients = new();
        private Dictionary<string, HashSet<DateTime>> _limitInfo = new();

        public DummyRepository()
        {
            
        }

        public DummyRepository(IRateLimiterSettings settings, IEnumerable<IClient> clients)
        {
            _rules.UnionWith(settings.Rules);

            foreach (var client in clients)
            {
                _clients.Add(client.Token, client);
            }
        }

        public (IClient, IEnumerable<IRequestLimitRule>) GetClientWithRules(Guid token, ResourcesType resource)
        {
            CheckClient(token);

            var client = _clients[token];
            var rules = _rules.Where(x => (x.RegionId == null || x.RegionId == client.RegionId) && (((byte)resource & (byte)x.ResourceType) > 0));

            return (client, rules);
        }

        public void SetRules(IEnumerable<IRequestLimitRule> rules)
        {
            _rules.UnionWith(rules);
        }
    
        public void AddClient(IClient client)
        {
            _clients.Add(client.Token, client);
        }

        public (int count, DateTime lastCalltime) GetClientLimitInfo(Guid token, DateTime utcTime, TimeSpan period, ResourcesType resource)
        {
            CheckClient(token);

            var key = KeyHelper.GetKey(token, resource);

            if (!_limitInfo.ContainsKey(key))
            {
                return (0, new DateTime());
            }

            var rules = _limitInfo[key].Where(x => x >= utcTime.Subtract(period) && x <= utcTime).Count();
            
            //assume that there are no data from the "future"
            var lastCall = _limitInfo[key].Max();

            return (rules, lastCall);
        }

        public void Incremet(Guid token, DateTime utcTime, ResourcesType resource)
        {
            var key = KeyHelper.GetKey(token, resource);

            if (_limitInfo.ContainsKey(key))
            {
                _limitInfo[key].Add(utcTime);
            }
            else
            {
                _limitInfo.Add(key, new HashSet<DateTime>() { utcTime });
            }
        }

        private void CheckClient(Guid token)
        {
            //TODO clarify about this case
            if (!_clients.ContainsKey(token))
            {
                throw new ArgumentException("Unknown client!");
            }
        }
    }    
}
