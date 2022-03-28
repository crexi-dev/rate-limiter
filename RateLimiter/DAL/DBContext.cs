using Microsoft.EntityFrameworkCore;
using RateLimiter.Models;
using System;
using System.Collections.Generic;

using System.Text;

namespace RateLimiter.DAL
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions<MyContext> options)
            :base (options)
            {
            }

        public DbSet<RateLimitRequest> Requests { get; set; }
        public DbSet<RateLimitResource> Resources { get; set; }
        public DbSet<RateLimitRule> Rules { get; set; }
        public DbSet<RateLimitRegion> Regions { get; set; }
    }
}
