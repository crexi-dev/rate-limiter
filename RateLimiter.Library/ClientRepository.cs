using System;
using System.Collections.Concurrent;

namespace RateLimiter.Library.Repository
{
    public class ClientRepository : IClientRepository {
        private ConcurrentDictionary<string, ClientRequestData> clientRequestHistory;

        public ClientRepository() {
            clientRequestHistory = new ConcurrentDictionary<string, ClientRequestData>();
        }

        public ClientRequestData GetClientData(string token) {
            return this.clientRequestHistory[token];
        }

        public void AddOrUpdate(string token, ClientRequestData clientData)
        {
            Func<string, ClientRequestData> addValue = (key) => { return clientData; };
            Func<string, ClientRequestData, ClientRequestData> updateValue = (key, clientRequestData) => { return clientData; };
            clientRequestHistory.AddOrUpdate(token, addValue, updateValue);
        }
    }
}