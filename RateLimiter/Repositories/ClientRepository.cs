using RateLimiter.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.Repositories
{
    public class ClientRepository : IClientRepository
    {
        public void Add(string clientType)
        {
            var existing = Store.Store.Clients.FirstOrDefault(x => x.ClientType == clientType);

            if (existing != null)
                throw new Exception($"Client with the Type:{clientType} already exist");

            Store.Store.Clients.Add(Client.Create(clientType));
        }

        public List<RateLimiterRule> GetRulesOfResource(string clientType, string path)
        {
            var client = Store.Store.Clients.FirstOrDefault(x => x.ClientType == clientType);

            if (client == null)
                throw new Exception($"Client with Type:{clientType} does not exist");

            var resource = client.Resources.FirstOrDefault(x => x.Path == path);

            if (resource == null)
                throw new Exception($"Resource with Path:{path} on Client:{clientType} does not exist");

            return resource.Rules;
        }

        public void AddResource(string clientType, Resource resource)
        {
            var client = Store.Store.Clients.FirstOrDefault(x => x.ClientType == clientType);

            if (client == null)
                throw new Exception($"Client with Type:{clientType} does not exist");

            client.AddResource(resource);
        }

        public void AddResources(string clientType, List<Resource> resources)
        {
            foreach (var resource in resources)
            {
                AddResource(clientType, resource);
            }
        }
    }
}
