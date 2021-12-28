using RateLimiter.DAL;
using RateLimiter.Entities;
using System.Linq;

namespace RateLimiter.BLL
{
    public static class ClientHelper
    {
        public static ClientModel GetClientByToken(string token)
        {
            return DatabaseSimulator.Clients.FirstOrDefault(x => x.Token == token);
        }
    }
}
