using System.Collections.Concurrent;
using RateLimiter.Library.Repository;

namespace RateLimiter.Library.Repository
{
    public class ClientRepository : IClientRepository {
        private ConcurrentDictionary<string, ClientRequestData> clientRequestHistory;

        public ClientRepository() {
            clientRequestHistory = new ConcurrentDictionary<string, ClientRequestData>();
        }

        public dynamic GetClientData(string token) {
            return this.clientRequestHistory[token];
        }
    }
}