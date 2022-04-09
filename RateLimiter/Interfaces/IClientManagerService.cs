using RateLimiter.DataStorageSimulator;

namespace RateLimiter.Interfaces
{
    public interface IClientManagerService
    {
        /// <summary>
        /// Maps Token to Rated Client.
        /// Returns UnRated Client if the Token represents un-Rated client
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Client GetRatedClientIdByToken(Token token);        
    }
}
