using System;

namespace RateLimiter.Library.Repository
{
    public interface IClientRepository {
        ClientRequestData GetClientData(string token);
        void UpdateClient(string token, ClientRequestData clientRequestData);
    }    
}