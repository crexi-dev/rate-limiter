using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.Model
{
    public class Client
    {
        public Client(string clientType)
        {
            ClientType = clientType;
            Resources = new List<Resource>();
        }

        public string ClientType { get; private set; }

        public List<Resource> Resources { get; private set; }

        public void AddResource(Resource resource)
        {
            var existing = Resources.FirstOrDefault(x => x.Path == resource.Path);

            if (existing != null)
                throw new System.Exception($"Resource with Path:{resource.Path} on Client:{ClientType} already exist");

            Resources.Add(resource);
        }

        public static Client Create(string clientType) => new Client(clientType);
    }
}
