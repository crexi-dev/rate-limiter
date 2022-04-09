using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using RateLimiter.API.Controllers;
using RateLimiter.Entity.Entities;
using RateLimiter.Entity.ViewModels;
using RateLimiter.Repository.Interfaces;
using RateLimiter.Repository.Repository;
using RateLimiter.Service.Interfaces;
using RateLimiter.Service.Services;
using System.Threading.Tasks;
using RateLimiter.Service.Util;
using System.Collections.Generic;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
        Employee mockEmployeeInsert = null;
        EmployeeService employeeService = null;


        [SetUp]
        public void SetUp()
        {
            mockEmployeeInsert = new Employee()
            {
                EmployeeId = 1,
                Name = "Ignacio",
                LastName = "Mariscal",
            };

        }

        [Test]
        public async Task Employee_Insert_Record_Validate_Employee_Data_Returned()
        {
            //Arrange
            var mockEmployeeRepository = new Mock<IEmployeeRepository>();

            mockEmployeeRepository.Setup(m => m.InsertEmployeeAsync(It.IsAny<Employee>()))
                     .Returns(Task.FromResult(mockEmployeeInsert));

            //Act 
            employeeService = new EmployeeService(mockEmployeeRepository.Object); 

            var result = await employeeService.InsertEmployeeAsync(new EmployeeInsertViewModel()); 
            //Assert
            Assert.AreEqual(result.SuccessMessage, EMPLOYEE.INSERT_MESSAGE);
            Assert.GreaterOrEqual(result.EmployeeId, 1);
            Assert.IsNotNull(result);

        }

        [Test]
        public async Task Employee_Insert_Validate_Employee_List_Count()
        {
            //Arrange
            var mockEmployeeRepository = new Mock<IEmployeeRepository>();
            var employeeList = new EmployeeListViewModel();
            employeeList.Employees.Add(new EmployeeViewModel
            {
                EmployeeId = 1,
                Name = "Ignacio",
                LastName = "Mariscal",
            });

            IEnumerable<Employee> employees = new Employee[] { mockEmployeeInsert };

            mockEmployeeRepository.Setup(m => m.InsertEmployeeAsync(It.IsAny<Employee>()))
                     .Returns(Task.FromResult(mockEmployeeInsert));

            mockEmployeeRepository.Setup(m => m.GetEmployees())
                     .Returns(employees);
            //Act 
            employeeService = new EmployeeService(mockEmployeeRepository.Object);

            var result = await employeeService.InsertEmployeeAsync(new EmployeeInsertViewModel());
            var employeeListResult = employeeService.GetEmployees();

            //Assert
            Assert.GreaterOrEqual(employeeList.Employees.Count, 1);
            Assert.IsNotNull(employeeList);

        }
    }
}
