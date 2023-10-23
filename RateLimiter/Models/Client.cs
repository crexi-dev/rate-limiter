using RateLimiter.Contracts;
using System;

namespace RateLimiter.Models
{
    public class Client : IClient
    {
        public Client(Guid token, string name, int regionId)
        {
            Token = token;
            Name = name;
            RegionId = regionId;
        }

        public Guid Token { get; init; }

        public string Name { get; init; }

        public int RegionId { get; init; }
    }
}
