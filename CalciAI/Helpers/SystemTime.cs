using System;

namespace CalciAI.Helpers
{
    public sealed class SystemTime
    {
        private static readonly TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        private SystemTime()
        {

        }

        public static readonly Func<DateTime> Now = () => DateTime.UtcNow;
        public static readonly Func<DateTime> Today = () => DateTime.UtcNow.Date;
        public static readonly Func<long> UtcUnixNow = () => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        public static readonly Func<long> UnixNow = () => DateTimeOffset.Now.ToUnixTimeMilliseconds();

        public static readonly Func<DateTime> IndiaNow = () => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
        public static readonly Func<DateTime> IndiaToday = () => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.Date, INDIAN_ZONE);

        public static long GetTimeStamp() => (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

    }

    public static class DateTimeHelper
    {
        public static DateTime ChangeTime(this DateTime dateTime, int hours, int minutes, int seconds, int milliseconds, DateTimeKind kind)
        {
            return new DateTime(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                hours,
                minutes,
                seconds,
                milliseconds,
                kind);
        }
        public static DateTime EquivalentWeekDay(int dayOfWeek)
        {
            int num = dayOfWeek;
            int num2 = (int)DateTime.Today.DayOfWeek;
            return DateTime.Today.AddDays(num - num2);
        }
    }
}