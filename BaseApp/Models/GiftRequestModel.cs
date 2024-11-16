using System.ComponentModel.DataAnnotations.Schema;

namespace BaseApp.Models
{
    [Table("gift_request")]
    public class GiftRequestModel
    {

        [Column("emp_id")]
        public long EmpId { get; set; }

        [Column("gift_id")]
        public long GiftId { get; set; }

        [Column("status")]
        public string Status { get; set; }

        public EmployeeModel Employee { get; set; }

        public GiftModel Gift { get; set; } 


    }
}
