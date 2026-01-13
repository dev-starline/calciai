using System;

namespace CalciAI.Persistance
{
    public static class DateConverter
    {
        //public static async Task<string> GetTimeZone(string operatorID)
        //{
        //    SqlParameter[] parameters =
        //    {
        //        new SqlParameter("@OperatorID", operatorID),
        //    };

        //    var data = await SqlService.ExecuteReaderAsync<TimeZoneModel>(SPNames.TimeZone_GetDetails, parameters);

        //    return data.AltName;
        //}

        public static DateTime? ConvertToUtcDate(DateTime? dateTime, string timeZone)
        {
            if (dateTime != null)
            {
                TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
                return TimeZoneInfo.ConvertTimeToUtc((DateTime)dateTime, tz);
            }
            else
            {
                return null;
            }
        }

        public static TimeSpan? ConvertToUtcTime(TimeSpan? timeSpan, string timeZone)
        {
            if (timeSpan != null)
            {
                string sdate = DateTime.Now.ToShortDateString();
                
                DateTime? dt = Convert.ToDateTime(sdate).Date + timeSpan;

                TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
                var utcTime = TimeZoneInfo.ConvertTimeToUtc((DateTime)dt, tz);

                return utcTime.TimeOfDay;
            }
            else
            {
                return null;
            }
        }

        public static TimeSpan? ConvertFromUtcTime(TimeSpan? timeSpan, string timeZone)
        {
            if (timeSpan != null)
            {
                string sdate = DateTime.Now.ToShortDateString();

                DateTime? dt = Convert.ToDateTime(sdate).Date + timeSpan;

                TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
                var utcTime = TimeZoneInfo.ConvertTimeFromUtc((DateTime)dt, tz);

                return utcTime.TimeOfDay;
            }
            else
            {
                return null;
            }
        }
    }
}