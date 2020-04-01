using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Interfaces
{
    public interface IRateLimiter
    {
        bool Validate(string ApiName, IClientRequest request);

        List<IClientRequest> RequestsLog { get; }
    }
}
