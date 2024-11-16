using System.ComponentModel.DataAnnotations.Schema;

namespace BaseApp.Models
{
    [Table("device")]
    public class DeviceModel : BaseModel
    {
        [Column("device_uuid")]
        public string UUID { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("emp_id")]
        public long EmpId { get; set; }

        public EmployeeModel Employee { get; set; }

    }
}
