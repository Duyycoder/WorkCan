using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseApp.Models
{
    [Table("task")]
    public class TaskModel : BaseModel
    {
        [Column("contents"), Required]
        public string Contents { get; set; }

        [Column("start_date"), Required]
        public DateTime StartDate { get; set; }

        [Column("end_date"), Required]
        public DateTime EndDate { get; set; }

        [Column("estimate"), Required]
        public int Estimate { get; set; }

        [Column("status"), Required]
        public string Status { get; set; }

        [Column("note")]
        public string Note { get; set; }

        [Column("project_id")]
        public long ProjectId { get; set; }

        public ProjectModel Project { get; set; }

        [Column("emp_id")]
        public long EmpId {  get; set; }

        public EmployeeModel Employee { get; set; }

    }
}
