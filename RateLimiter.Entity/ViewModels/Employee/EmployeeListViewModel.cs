using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Entity.ViewModels
{
    public class EmployeeListViewModel : BaseViewModel
    {
        public EmployeeListViewModel()
        {
            Employees = new List<EmployeeViewModel>();
        }
        public List<EmployeeViewModel> Employees { get; set; } 

    }
}
