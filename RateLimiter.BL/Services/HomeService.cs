using RateLimiter.BL.ServicesInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.BL.Services
{
    public class HomeService : IHomeService
    {
        public string GetHomeAnswer()
        {
            return "Hello";
        }
    }
}
