using System;

namespace SSCMS.Comments.Utils
{
    public static class CommentUtils
    {
        public const string TableName = "sscms_comments";

        public static DateTime ToDateTime(string dateTimeStr)
        {
            return ToDateTime(dateTimeStr, DateTime.Now);
        }

        public static DateTime ToDateTime(string dateTimeStr, DateTime defaultValue)
        {
            var datetime = defaultValue;
            if (!string.IsNullOrEmpty(dateTimeStr))
            {
                if (!DateTime.TryParse(dateTimeStr.Trim(), out datetime))
                {
                    datetime = defaultValue;
                }
                return datetime;
            }
            if (datetime <= DateTime.MinValue)
            {
                datetime = DateTime.Now;
            }
            return datetime;
        }

        public static int ToIntWithNegative(string intStr, int defaultValue)
        {
            if (!int.TryParse(intStr?.Trim(), out var i))
            {
                i = defaultValue;
            }
            return i;
        }

        public static decimal ToDecimalWithNegative(string intStr, decimal defaultValue)
        {
            if (!decimal.TryParse(intStr?.Trim(), out var i))
            {
                i = defaultValue;
            }
            return i;
        }
    }
}
