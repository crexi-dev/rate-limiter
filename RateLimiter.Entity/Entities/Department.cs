using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Entity.Entities
{
    [Table("department")]
    public class Department
    {
        [Key]
        [Column("department_id", TypeName = "int")]
        public int DepartmentId { get; set; }

        [Required]
        [Column("name", TypeName = "nvarchar")]
        [MaxLength(100), MinLength(5)]
        public string Name { get; set; }
    }
}
