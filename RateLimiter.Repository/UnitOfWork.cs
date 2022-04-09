using RateLimiter.Entity.Entities;
using RateLimiter.Repository.Context;
using RateLimiter.Repository.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.Repository
{
    public class UnitOfWork : IDisposable
    {
        private RateLimiterContext context = new RateLimiterContext();
        private RateLimiterRepository<Department> departmentRepository;

        public RateLimiterRepository<Department> DepartmentRepository
        {
            get
            {

                if (this.departmentRepository == null)
                {
                    this.departmentRepository = new RateLimiterRepository<Department>(context);
                }
                return departmentRepository;
            }
        }

        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
