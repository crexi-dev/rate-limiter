using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Entity.ViewModels.Department
{
    public class DepartmentViewModel : BaseViewModel
    {
        public int DepartmentId { get; set; }

        public string Name { get; set; }
    }
}
