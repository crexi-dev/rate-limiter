using Microsoft.AspNetCore.Mvc;
using RateLimiter.Service.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RateLimiter.Entity.ViewModels;

namespace RateLimiter.API.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class EmployeeController : Controller
    {

        private readonly IEmployeeService _employeeService;
        private readonly ILogger<EmployeeController> _logger;
         

        public EmployeeController(IEmployeeService employeeService,
                                 ILogger<EmployeeController> logger)
        {
            _employeeService = employeeService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetEmployees()
        {             
            _logger.LogInformation($"EmployeeController.GetEmployees action executed at {DateTime.UtcNow.ToLongTimeString()}");
            var employees = _employeeService.GetEmployees();
            return Ok(employees);
        }


        [HttpDelete] 
        public async Task<EmployeeViewModel> DeleteEmployee(int employeeId)
        {
            _logger.LogInformation($"EmployeeController.DeleteEmployee  action executed at {DateTime.UtcNow.ToLongTimeString()}");
            return await _employeeService.DeleteEmployeeAsync(employeeId);
        }

        [HttpPost]
        public async Task<EmployeeViewModel> AddEmployee([FromBody] EmployeeInsertViewModel employeeViewModel)
        {
            _logger.LogInformation($"EmployeeController.AddEmployee  action executed at {DateTime.UtcNow.ToLongTimeString()}");
            return await _employeeService.InsertEmployeeAsync(employeeViewModel);
        }
    }
}
