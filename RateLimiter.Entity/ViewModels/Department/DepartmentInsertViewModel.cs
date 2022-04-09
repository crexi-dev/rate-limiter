using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Entity.ViewModels.Department
{
    public  class DepartmentInsertViewModel
    {
        [Required]
        [MinLength(5, ErrorMessage = "Department Name is Required")]
        public string Name { get; set; }
    }
}
