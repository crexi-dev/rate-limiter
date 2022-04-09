using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Entity.ViewModels
{
    public class EmployeeViewModel : BaseViewModel
    {
        public int EmployeeId { get; set; }
         
        public string Name { get; set; }

        public string LastName { get; set; }

        public string FullName
        {
            get
            {
                return $"{Name} {LastName}";
            }
        }
    }
}
