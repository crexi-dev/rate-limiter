using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RateLimiter.Entity.Entities;

namespace RateLimiter.Repository.Interfaces
{
    public  interface IEmployeeRepository : IDisposable
    {
        IEnumerable<Employee> GetEmployees();
        Task<Employee> GetEmployeeByIdAsync(int employeeId);
        Task<Employee> InsertEmployeeAsync(Employee employee);
        Task<Employee> DeleteEmployeeAsync(int employeeId);
        //void UpdateEmployee(Employee employee);
        Task<int> SaveAsync();
    }
}
