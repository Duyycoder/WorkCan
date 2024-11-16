using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseApp.Models
{

    [Table("point")]
    public class PointModel : BaseModel
    {
        [Column("point")]
        public double Point { get; set; }

        [Column("emp_id"), ForeignKey(nameof(EmployeeModel.Id))]
        public long EmpId { get; set; }

        public EmployeeModel Employee { get; set; } 

    }

}
