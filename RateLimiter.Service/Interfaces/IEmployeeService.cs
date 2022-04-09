using RateLimiter.Entity.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Service.Interfaces
{
    public interface IEmployeeService
    {
        EmployeeListViewModel GetEmployees();
        Task<EmployeeViewModel> GetEmployeeByIdAsync(int employeeId);
        Task<EmployeeViewModel> InsertEmployeeAsync(EmployeeInsertViewModel employee);
        Task<EmployeeViewModel> DeleteEmployeeAsync(int employeeId);
    }
}
