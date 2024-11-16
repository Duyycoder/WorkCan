using BaseApp.Constants;
using BaseApp.Models;

namespace BaseApp.DTO
{
    public class ResponseActivitiesInDayDTO
    {

        public ResponseActivity EarliestCheckIn { get; set; }

        public ResponseActivity LastestCheckOut { get; set; }

        public List<ResponseActivity> BreakStartList { get; set; }

        public List<ResponseActivity> BreakEndList { get; set; }

    }

    public class ResponseActivity
    {
        public long Id { get; set; }

        public EnumTypes.ActivityType ActivityType { get; set; }

        public TimeOnly ActivityTime { get; set; }  

        public double Longtitude { get; set; }

        public double Latitude { get; set; }    

    }

}
