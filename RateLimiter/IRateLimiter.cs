using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
    public interface IRateLimiter
    {
        bool Check(string resource, string token);
    }
}
