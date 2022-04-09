using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RateLimiter.Entity.Entities
{
    [Table("employee")]
    public class Employee
    {
        [Key]
        [Column("employee_id", TypeName = "int")]
        public int EmployeeId { get; set; }
 
        [Required]
        [Column("name", TypeName = "nvarchar")]
        [MaxLength(100), MinLength(5)]
        public string Name { get; set; }

        [Required]
        [MaxLength(100), MinLength(5)]
        [Column("last_name", TypeName = "nvarchar")]
        public string LastName { get; set; }
 
    }
}
