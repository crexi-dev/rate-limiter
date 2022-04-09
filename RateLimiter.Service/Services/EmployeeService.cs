using RateLimiter.Entity.ViewModels;
using RateLimiter.Repository.Interfaces;
using RateLimiter.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using RateLimiter.Entity.Entities;
using RateLimiter.Service.Util;

namespace RateLimiter.Service.Services
{
    public class EmployeeService : IEmployeeService
    {
        private IEmployeeRepository employeeRepository;
 

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            this.employeeRepository = employeeRepository;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        public async Task<EmployeeViewModel> DeleteEmployeeAsync(int employeeId)
        {
            var employeeViewModel = new EmployeeViewModel();
            try
            {
                var employee = await employeeRepository.GetEmployeeByIdAsync(employeeId);

                if (employee == null)
                {
                    employeeViewModel.ErrorMessage = EMPLOYEE.DOES_NOT_EXIST;
                }
                else
                {
                    employeeViewModel = MapEmployeViewModel(employee);
                    await employeeRepository.DeleteEmployeeAsync(employeeId);
                    await employeeRepository.SaveAsync();
                    employeeViewModel.SuccessMessage = EMPLOYEE.DELETE_MESSAGE;
                }           
            }
            catch (Exception ex)
            {
                employeeViewModel.ErrorMessage = ex.Message;
            }

            return employeeViewModel;

        }


        /// <summary>
        /// Get an employee by Id 
        /// </summary>
        /// <param name="employeeId">Employee Id</param>
        /// <returns>Employee view Model</returns>
        public async Task<EmployeeViewModel> GetEmployeeByIdAsync(int employeeId)
        {
            var employeeViewModel = new EmployeeViewModel();
            try
            {
                var employee = await employeeRepository.GetEmployeeByIdAsync(employeeId);

                if (employee == null)
                {
                    employeeViewModel.ErrorMessage = EMPLOYEE.DOES_NOT_EXIST;
                }
                else
                {
                    employeeViewModel = MapEmployeViewModel(employee);                   
                }
            }
            catch (Exception ex)
            {
                employeeViewModel.ErrorMessage = ex.Message;
            }

            return employeeViewModel;
        }

       
        /// <summary>
        /// Get a List of employees in the DB 
        /// </summary>
        /// <returns>List of employeview Model</returns>
        public EmployeeListViewModel GetEmployees()
        {
            var employeeList = new EmployeeListViewModel();
            try
            {
                var employees = employeeRepository.GetEmployees();

                var employeesVM = from e in employees
                                  select new EmployeeViewModel()
                                  {
                                      EmployeeId = e.EmployeeId,
                                      Name = e.Name,
                                      LastName = e.LastName
                                  };

                employeeList.Employees = employeesVM.ToList();
                employeeList.SuccessMessage = EMPLOYEE.LIST_MESSAGE;
            } 
            catch(Exception ex)
            {
                employeeList.ErrorMessage = ex.Message;
            } 
            
            return employeeList;
        }

        /// <summary> 
        /// Insert into employee table
        /// </summary>
        /// <param name="employee"> Entity Employe DBSET</param>
        /// <returns>Employeee View Model </returns>
        public async Task<EmployeeViewModel> InsertEmployeeAsync(EmployeeInsertViewModel employee)
        {
            var employeeViewModel = new EmployeeViewModel();
            try
            {

                /// Perform any validation on the view model before to insert the record in the database.
                /// 


                /// we can use automapper instead to do ti manually
                var employeeDb = new Employee
                {
                    LastName = employee.LastName,
                    Name = employee.Name
                };

                var employeeResult = await employeeRepository.InsertEmployeeAsync(employeeDb);
                await employeeRepository.SaveAsync();

                employeeViewModel = MapEmployeViewModel(employeeResult);
                employeeViewModel.SuccessMessage = EMPLOYEE.INSERT_MESSAGE;
            }
            catch (Exception ex)
            {
                employeeViewModel.ErrorMessage = ex.Message;
            }

            return employeeViewModel;
        }

        /// <summary>
        /// Return a new employee view model using as base line employe db entity
        /// </summary>
        /// <param name="employee"> Entity Employe DBSET</param>
        /// <returns>Employeee View Model </returns>
        private EmployeeViewModel MapEmployeViewModel(Employee employee)
        {
            return new EmployeeViewModel()
            {
                EmployeeId = employee.EmployeeId,
                Name = employee.Name,
                LastName = employee.LastName
            };
        }
    }
}
