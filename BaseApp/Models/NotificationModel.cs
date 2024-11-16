using System.ComponentModel.DataAnnotations.Schema;

namespace BaseApp.Models
{
    [Table("notification")]
    public class NotificationModel : BaseModel
    {
        [Column("title")]
        public string Title { get; set; }

        [Column("contents")] 
        public string Contents { get; set; }

        [Column("notification_date_time")]
        public DateTime NotificationDateTime { get; set; }

        [Column("repeat_duration")]
        public int RepeatDuration { get; set; }

        [Column("emp_id"), ForeignKey(nameof(EmployeeModel.Id))]
        public long EmpId { get; set; } 

        public EmployeeModel Employee { get; set; }

    }
}
