using RateLimiter.Entity.ViewModels;
using RateLimiter.Entity.ViewModels.Department;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Service.Interfaces
{
    public interface IDepartmentService
    {
        DepartmentListViewModel GetDepartments();
        DepartmentViewModel GetDepartmentById(int departmentId);
        DepartmentViewModel InsertDepartment(DepartmentInsertViewModel department);
        DepartmentViewModel DeleteDepartment(int departmentId);
    }
}
