using System;
using System.Globalization;

namespace CalciAI.Persistance
{
    public static class DateTimeConverter
    {
        public static DateTime ConvertTimeToTimeZone(DateTime iDate, string altName)
        {
            iDate = TimeZoneInfo.ConvertTime(iDate, TimeZoneInfo.FindSystemTimeZoneById(altName));
            return iDate;
        }

        public static DateTime ConvertUTCTimeToTimeZone(DateTime iDate, string altName)
        {
            iDate =  TimeZoneInfo.ConvertTimeFromUtc(iDate, TimeZoneInfo.FindSystemTimeZoneById(altName));
            return iDate;
        }

        public static DateTime ConvertTimeToTimeZoneMilliSeconds(DateTime iDate, string altName)
        {
            iDate = TimeZoneInfo.ConvertTime(iDate, TimeZoneInfo.FindSystemTimeZoneById(altName));
            return iDate;
        }

        public static long ConvertToSystemTimeZone(long currentTime)
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(currentTime);
            DateTime dateTime = dateTimeOffset.DateTime;
            dateTime = ConvertTimeToTimeZoneMilliSeconds(dateTime , "");
            return (long)ConvertToUnixTimestamp(dateTime);
        }

        public static DateTime ConvertToSystemTimeZoneTrueDataTick(long currentTime, string altName)
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(currentTime);
            DateTime toReturnDate = TimeZoneInfo.ConvertTimeFromUtc(dateTimeOffset.DateTime, TimeZoneInfo.FindSystemTimeZoneById(altName));
            return toReturnDate;
          
        }

        public static DateTime StringToTime(string time)
        {
            DateTime dateTime = DateTime.ParseExact(time, "HH:mm:ss",
                                       CultureInfo.InvariantCulture);
            /*Array arrTime = time.Split(":");
            if(int.Parse(arrTime.GetValue(0).ToString()) >= 18)
            {
                if (int.Parse(arrTime.GetValue(1).ToString()) >= 30)
                {
                    dateTime = dateTime.Subtract(TimeSpan.FromDays(1));
                }
            }*/
            return dateTime;
        }

        public static double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalMilliseconds);
        }

        public static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return origin.AddSeconds(timestamp);
        }
    }
}
