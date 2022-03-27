using RateLimiterMy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiterMy.Contracts
{
    public interface IRequest
    {
        string Controler { get; }
        string Method { get; }
        string Token { get;  }
        Location Location { get;}
        DateTime Time{ get; }
    }
}
