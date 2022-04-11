using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RateLimiter.Entity.ViewModels.Department;
using RateLimiter.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RateLimiter.API.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class DepartmentController : Controller
    {

        private readonly IDepartmentService _departmentService;
        private readonly ILogger<DepartmentController> _logger;


        public DepartmentController(IDepartmentService departmentService,
                                 ILogger<DepartmentController> logger)
        {
            _departmentService = departmentService;
            _logger = logger;
        }

        [HttpGet] 
        public IActionResult GetDeparments()
        {
            _logger.LogInformation($"DepartmentController.GetDeparments action executed at {DateTime.UtcNow.ToLongTimeString()}");
            var employees = _departmentService.GetDepartments();
            return Ok(employees);
        }


        [HttpDelete]
        public DepartmentViewModel DeleteDepartment(int deparmentId)
        {
            _logger.LogInformation($"DepartmentController.DeleteDepartment  action executed at {DateTime.UtcNow.ToLongTimeString()}");
            return _departmentService.DeleteDepartment(deparmentId);
        }

        [HttpPost]
        public DepartmentViewModel AddDepartment([FromBody] DepartmentInsertViewModel departmentViewModel)
        {
            _logger.LogInformation($"DepartmentController.AddDepartment  action executed at {DateTime.UtcNow.ToLongTimeString()}");
            return _departmentService.InsertDepartment(departmentViewModel);
        }

    }
}
