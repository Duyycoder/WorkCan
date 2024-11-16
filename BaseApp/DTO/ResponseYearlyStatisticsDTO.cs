namespace BaseApp.DTO
{
    public class ResponseYearlyStatisticsDTO
    {

        public int CountAnnualLeave {  get; set; }

        public int CountLateOrLeaveEarly { get; set; }

        public string TotalLateOrLeaveEarly { get; set; }

        public string Overtime { get; set; }

        public int CountRemoteDays { get; set; }

        public List<string> DetailLeaveApplicationList { get; set; }

        public List<string> DetailOvertimeApplicationList { get; set; }

    }

}
