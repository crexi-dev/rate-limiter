using System;

namespace RateLimiter.Repository
{
    public interface IClientRepository {
        dynamic GetClientData(string token);
    }    
}