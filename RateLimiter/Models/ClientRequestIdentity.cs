using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Models
{
    public class ClientRequestIdentity : IClientRequestIdentity
    {
        public string ClientId { get; init; }
        public string RegionPrefix { get; init; }
    }
}
