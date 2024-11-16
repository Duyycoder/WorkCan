using System.ComponentModel.DataAnnotations.Schema;

namespace BaseApp.Models
{
    [Table("location")]
    public class LocationModel : BaseModel
    {

        [Column("device_id")]
        public string DeviceId { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("longtitude")]
        public double Longtitude { get; set; }

        [Column("latitude")]
        public double Latitude { get; set; }

        [Column("emp_id")]
        public long EmpId { get; set; }

        public EmployeeModel Employee { get; set; }

    }
}
