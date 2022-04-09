using RateLimiter.Entity.Entities;
using RateLimiter.Repository.Context;
using RateLimiter.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace RateLimiter.Repository.Repository
{
    public class EmployeeRepository : IEmployeeRepository, IDisposable
    {
        private RateLimiterContext context;

        public EmployeeRepository(RateLimiterContext context)
        {
            this.context = context;
        }

        public IEnumerable<Employee> GetEmployees()
        {
            var employees = context.Employees.ToList();
            return employees;
        }

        public async  Task<Employee> GetEmployeeByIdAsync(int employeeId)
        {
            return await context.Employees.FindAsync(employeeId);
        }

        public async Task<Employee> InsertEmployeeAsync(Employee employee)
        {
            await context.Employees.AddAsync(employee);
            return employee;
        }

        public async Task<Employee> DeleteEmployeeAsync(int employeeId)
        {
            var employee = await context.Employees.FindAsync(employeeId);
            context.Employees.Remove(employee);
            return employee;
        }

        //public void UpdateEmployee(Employee employee)
        //{
 
        //    context.Employees(employee).State = EntityState.Modified;
        //}

        public async Task<int> SaveAsync()
        {
           return await context.SaveChangesAsync();
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
