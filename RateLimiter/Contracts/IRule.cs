using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiterMy.Contracts
{
    public interface IRule
    {
        bool Validate(IRequest request);
    }
}
