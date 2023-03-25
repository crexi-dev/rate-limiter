using RateLimiter.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
    public class ClientInfoService : IResource
    {
        public string GetResource(string request, string clientId)
        {
            return String.Format("You've got client info {0} for client {1}", request, clientId);
        }
    }
}
