namespace BaseApp.Helpers
{
    public class DateTimeHelper
    {

        public class ParseResult
        {
            public DateTime? DateTime { get; set; }
            public TimeOnly? TimeOnly { get; set; }
        }


        public ParseResult TryParseDateTimeOrTimeOnly(string input)
        {
            var result = new ParseResult();

            // Kiểm tra xem có thể chuyển đổi thành DateTime
            if (DateTime.TryParse(input, out DateTime dateTime))
            {
                result.DateTime = dateTime;
            }

            // Kiểm tra xem có thể chuyển đổi thành TimeOnly
            if (TimeOnly.TryParse(input, out TimeOnly timeOnly))
            {
                result.TimeOnly = timeOnly;
            }

            return result;
        }

    }
}
