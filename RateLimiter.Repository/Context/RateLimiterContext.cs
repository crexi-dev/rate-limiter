using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using RateLimiter.Entity.Entities;

namespace RateLimiter.Repository.Context
{
    public class RateLimiterContext : DbContext
    {
        public RateLimiterContext(DbContextOptions<RateLimiterContext> options)
              : base(options)
        {
        }

        public RateLimiterContext( )
           : base( )
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }

    }
}
