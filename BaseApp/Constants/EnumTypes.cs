namespace BaseApp.Constants
{
    public class EnumTypes
    {

        public enum ShiftStatus
        {
            LATE,
            OUTOFOFFICE,
            LEAVE,
            NA
        }

        public enum ActivityType
        {
            CHECKIN,
            CHECKOUT,
            BREAKSTART,
            BREAKEND
        }

        public enum ApplicationType
        {
            LEAVE,
            HOLIDAY,
            OVERTIME,
            EARLYLEAVE,
            GOINGOUT,
            REMOTE
        }

        public enum ApplicationStatus
        {
            REQUESTED,
            APPROVED,
            REJECTED,
            CANCELLED
        }

        public enum RoleType
        {
            ROLE_ADMIN,
            ROLE_EMPLOYEE
        }

    }
}
