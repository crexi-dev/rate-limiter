using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Entity.ViewModels.Department
{
    public class DepartmentListViewModel : BaseViewModel
    {
        public List<DepartmentViewModel> Departments { get; set; }
    }
}
