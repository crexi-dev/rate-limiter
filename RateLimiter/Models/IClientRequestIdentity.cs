using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Models
{
    public interface IClientRequestIdentity
    {
        string ClientId { get; }

        string RegionPrefix { get; }
    }
}
