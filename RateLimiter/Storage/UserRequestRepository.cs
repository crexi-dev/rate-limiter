using RateLimiter.Domain;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RateLimiter.Storage
{
    public sealed class UserRequestRepository : IUserRequestRepository
    {
        private ConcurrentDictionary<string, List<UserRequest>> userRequestStorage = new();

        public UserRequestRepository()
        {
        }

        internal UserRequestRepository(ConcurrentDictionary<string, List<UserRequest>> userRequests)
        {
            userRequestStorage = userRequests;
        }

        public Task AddOrUpdateAsync(UserRequest userRequest)
        {
            if (userRequest == null)
                throw new ArgumentNullException(nameof(userRequest));

            if (userRequestStorage.TryGetValue(userRequest.AccessToken, out var existingUserRequests))
            {
                existingUserRequests ??= new List<UserRequest>();
                existingUserRequests.Add(userRequest);
            }
            else
            {
                userRequestStorage[userRequest.AccessToken] = new List<UserRequest> { userRequest };
            }

            return Task.CompletedTask;
        }

        public Task<IEnumerable<UserRequest>> GetAllAsync(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
                throw new ArgumentNullException(nameof(accessToken));

            userRequestStorage.TryGetValue(accessToken, out var userRequests);

            var resultSet = userRequests ?? new List<UserRequest>();

            return Task.FromResult(resultSet.AsEnumerable());
        }
    }
}
