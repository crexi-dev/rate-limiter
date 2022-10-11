using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace RateLimiter
{
    internal class RequestsHistory
    {
        private readonly ConcurrentDictionary<string, List<DateTimeOffset>> _usersRequests = new();

        public void AcceptRequest(string userToken, DateTimeOffset requestDateTime)
        {
            var userRequests = _usersRequests.GetOrAdd(userToken, new List<DateTimeOffset>());
            userRequests.Add(requestDateTime);
        }

        public IReadOnlyList<DateTimeOffset> GetRequests(string userToken)
        {
            return _usersRequests.GetValueOrDefault(userToken, new());
        }
    }
}