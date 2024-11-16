using System.ComponentModel.DataAnnotations.Schema;

namespace BaseApp.Models
{
    [Table("company_info")]
    public class CompanyInfoModel : BaseModel
    {

        [Column("gps_infor")]
        public string GpsInfor { get; set; }

        [Column("address")]
        public string address { get; set; }

        [Column("longtitude")]
        public float longtitude { get; set; }

        [Column("latitude")]
        public float latitude { get; set; }

        [Column("start_time")]
        public TimeOnly StartTime { get; set; }

        [Column("end_time")]
        public TimeOnly EndTime { get; set; }

        [Column("default_break_start")]
        public TimeOnly DefaultBreakStart { get; set; }

        [Column("default_break_end")]
        public TimeOnly DefaultBreakEnd { get; set; }

        

    }
}
