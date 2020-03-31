using System;

namespace RateLimiter.Library.Repository
{
    public interface IClientRepository {
        dynamic GetClientData(string token);
    }    
}