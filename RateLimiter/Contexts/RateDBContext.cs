using Microsoft.EntityFrameworkCore;
using RateLimiter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Contexts
{
    /// <summary>
    /// The caching mechanism should be key-value based.
    /// </summary>
    public class RateDBContext : RateDBContextBase
    {
        public RateDBContext(DbContextOptions<RateDBContextBase> options) : base(options)
        {
            
        }
    }
}
