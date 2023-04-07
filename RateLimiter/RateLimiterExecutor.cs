using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
    public class RateLimiterExecutor
    {
        private readonly LimiterResource _resource;
        private readonly IClientIdentifier _clientIdentifier;

        public RateLimiterExecutor(LimiterResource resource, IClientIdentifier clientIdentifier)
        {
            _resource = resource;
            _clientIdentifier = clientIdentifier;
        }

        public bool IsExceeded() 
        {
            return _resource.Rules.Any(x=>x.IsExceeded(GetKey()));
        }

        private string GetKey() 
        {
            return string.Concat(_clientIdentifier.Id, _resource.Name);
        }

    }
}
