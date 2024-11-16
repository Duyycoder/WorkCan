namespace BaseApp.Constants
{
    public class CommonConstants
    {
        public static readonly string SORT_ASC = "ASC";

        public static readonly string SORT_DESC = "DESC";

        public static readonly string ROLE_SUPER_ADMIN = "super_admin";

        public static readonly string ROLE_USER = "user";

        public static readonly string ROLE_ADMIN = "admin";

        public static readonly string SORT_BY_TIME = "create_date";

        public static readonly string FORMAT_DATE_PATTERN = "dd/MM/yyyy";

        public static readonly string FORMAT_DATE_PATTERN_DETAIL = "dd/MM/yyyy HH:mm:ss";

        public static int SIZE_OF_PAGE = 10;

        public static int DEFAULT_PAGE = 1;

        public static long MAX_FILE_SIZE_IN_BYTES = 10 * 1024 * 1024; // 10MB

        public static long MIN_FILE_SIZE_IN_BYTES = 1 * 1024; // 1KB

        public static string[] ALLOWED_FILE_TYPES = { "image/jpeg", "image/png", "image/jpg", "image/gif" };

        public static readonly double EARTH_RADIUS_BY_METER = 6371000; // Earth's radius in meters

    }
}
