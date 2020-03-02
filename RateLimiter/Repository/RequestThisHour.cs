using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Repository
{
    public interface IRequestThisHour
    {
        int GetRequestThisHour();
    }

    public class RequestThisHour : IRequestThisHour
    {
        public int GetRequestThisHour()
        {
            return 78;   
        }
    }
}
