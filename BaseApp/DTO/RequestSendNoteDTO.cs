using BaseApp.Constants;

namespace BaseApp.DTO
{
    public class RequestSendNoteDTO
    {

        public string Note { get; set; }

        public EnumTypes.ActivityType? Type { get; set; }

    }
}
