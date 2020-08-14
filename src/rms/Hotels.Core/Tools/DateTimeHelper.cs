using System;

namespace Hotels.Core.Tools
{
    public static class DateTimeHelper
    {
        public static int GetWeekNumber(this DateTime dateTime)
        {
            var dayOfWeek = (((int)dateTime.DayOfWeek) + 6) % 7;
            var dayOfYear = dateTime.DayOfYear - 1;

            return 1 + (dayOfYear / 7) + ((dayOfWeek < dayOfYear % 7) ? 1 : 0);
        }
    }
}
