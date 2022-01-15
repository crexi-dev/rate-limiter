using RateLimiter.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RateLimiter.Storage
{
    public interface IUserRequestRepository
    {
        Task<IEnumerable<UserRequest>> GetAllAsync(string accessToken);

        Task AddOrUpdateAsync(UserRequest userRequest);
    }
}
