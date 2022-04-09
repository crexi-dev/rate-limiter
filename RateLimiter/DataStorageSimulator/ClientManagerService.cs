using RateLimiter.Interfaces;

namespace RateLimiter.DataStorageSimulator
{
    /// <summary>
    /// Client manager class, basically maps token to client
    /// </summary>
    class ClientManagerService : IClientManagerService
    {
        public Client GetRatedClientIdByToken(Token token)
        {
            switch (token)
            {
                case Token.ClientAToken:
                    {
                        return Client.AClient;
                    }
                case Token.ClientBToken:
                    {
                        return Client.BClient;
                    }
                case Token.ClientCToken:
                    {
                        return Client.CClient;
                    }
            }
            return Client.UnRated;
        }
    }
}
