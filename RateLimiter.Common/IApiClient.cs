using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.Common
{
    public interface IApiClient
    {
        Guid ClientId { get; }

        string Region { get; }

        // Other properties as relevant from access token.
    }
}
