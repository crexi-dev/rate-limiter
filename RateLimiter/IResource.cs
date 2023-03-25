using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Resources
{
        public interface IResource
        {
            string GetResource(string request, string clientId);

        }
}
