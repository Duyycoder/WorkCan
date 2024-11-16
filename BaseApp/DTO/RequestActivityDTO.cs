using BaseApp.Constants;

namespace BaseApp.DTO
{
    public class RequestActivityDTO
    {
        public long EmpId { get; set; }

        public string ActivityTime { get; set; }

        public EnumTypes.ActivityType? Type { get; set; }

        public string DeviceId { get; set; }

        public double Longtitude { get; set; }

        public double Latitude { get; set; }
    }
}
