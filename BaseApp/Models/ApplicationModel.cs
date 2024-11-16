using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using BaseApp.Constants;

namespace BaseApp.Models
{
    [Table("application")]
    public class ApplicationModel : BaseModel
    {

        [Column("name")]
        public string Name { get; set; }

        [Column("type")]
        public EnumTypes.ApplicationType Type { get; set; }

        [Column("start_date")]
        public DateTime StartDate { get; set; }

        [Column("end_date")]
        public DateTime EndDate { get; set; }

        [Column("note")]
        public string Note { get; set; }

        [Column("status")] 
        public EnumTypes.ApplicationStatus Status {  get; set; }

        [Column("emp_id")]
        public long EmployeeId { get; set; }

        public EmployeeModel Employee { get; set; }

    }

}
