using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.RateLimitHandlers
{
    public class ProcessFactory : ProcessFactoryBase
    {
        public ProcessFactory(XRequestsPerTimespanHandler xRequestsPerTimespan,
                              LastCallHandler lastCallHandler,
                              USBasedTokensHandler usBasedTokensHandler,
                              EUBasedTokensHandler eUBasedTokensHandler) : base(xRequestsPerTimespan, 
                                                                                lastCallHandler, 
                                                                                usBasedTokensHandler, 
                                                                                eUBasedTokensHandler)
        {

        }
    }
}
