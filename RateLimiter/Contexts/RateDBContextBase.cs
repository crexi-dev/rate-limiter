using Microsoft.EntityFrameworkCore;
using RateLimiter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Contexts
{
    public class RateDBContextBase : DbContext
    {
        public RateDBContextBase(DbContextOptions<RateDBContextBase> options) : base(options)
        {

        }

        DbSet<RequestHistoryEModel> RequestHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RequestHistoryEModel>()
                        .HasKey(nameof(RequestHistoryEModel.Id));
        }
    }
}
