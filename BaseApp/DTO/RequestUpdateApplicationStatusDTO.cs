using BaseApp.Constants;

namespace BaseApp.DTO
{
    public class RequestUpdateApplicationStatusDTO
    {

        public long ApplicationId { get; set; }

        public EnumTypes.ApplicationStatus ApplicationStatus { get; set; }

    }
}
