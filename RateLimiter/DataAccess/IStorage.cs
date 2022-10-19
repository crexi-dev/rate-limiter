using RateLimiter.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.DataAccess
{
    public interface IStorage
    {
        ClientRequestStorage GetToken(string key);
        void SetToken(string key, ClientRequestStorage token);
    }
}
