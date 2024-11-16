namespace BaseApp.DTO
{
    public class ResponseAttendance
    {
        public string TotalWorktime { get; set; }

        public string TotalApprovedOvertime { get; set; }

        public string TotalPendingOvertime { get; set; }

        public string Break {  get; set; }

        public string TotalNightApprovedOvertime { get; set; }

        public string TotalApprovedHolidayWork { get; set; }

        public string TotalPoints {  get; set; }
        
        public List<ResponseDailyLog> ResponseDailyLogs { get; set; }

    }


    public class ResponseDailyLog
    {
        public string Day { get; set; } 

        public string CheckInTime { get; set; }

        public string CheckOutTime { get; set; }

        public string BreakTime { get; set; }

        public string WorkTime { get; set; }

        public string Status { get; set; }

        public bool isHoliday { get; set; }
    }

}
