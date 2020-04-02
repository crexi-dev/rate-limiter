using System;

namespace RateLimiter.Library.Repository
{
    public interface IClientRepository {
        ClientRequestData GetClientData(string token);
        void AddOrUpdate(string token, ClientRequestData clientData);
    }    
}