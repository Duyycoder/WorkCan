using BaseApp.Constants;

namespace BaseApp.DTO
{
    public class RequestApplicationDTO
    {
        public long EmpId {  get; set; }

        public string Title { get; set; }

        public EnumTypes.ApplicationType ApplicationType { get; set; }

        public DateTime StartedDate { get; set; }

        public DateTime EndedDate { get; set; }

        public string Note { get; set; }

    }
}
