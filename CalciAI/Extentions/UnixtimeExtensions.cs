using System;

namespace CalciAI.Extentions
{
    public static class UnixtimeExtensions
    {
        public static readonly DateTime UNIXTIME_ZERO_POINT = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Converts a Unix timestamp (UTC timezone by definition) into a DateTime object
        /// </summary>
        /// <param name="value">An input of Unix timestamp in seconds or milliseconds format</param>
        /// <param name="localize">should output be localized or remain in UTC timezone?</param>
        /// <param name="isInMilliseconds">Is input in milliseconds or seconds?</param>
        /// <returns></returns>
        public static DateTime? FromUnixtime(this long value, bool localize = false, bool isInMilliseconds = true)
        {
            if (value == 0)
            {
                return null;
            }

            DateTime result = isInMilliseconds ? UNIXTIME_ZERO_POINT.AddMilliseconds(value) : UNIXTIME_ZERO_POINT.AddSeconds(value);

            return localize ? result.ToLocalTime() : result;
        }

        /// <summary>
        /// Converts a DateTime object into a Unix time stamp
        /// </summary>
        /// <param name="value">any DateTime object as input</param>
        /// <param name="isInMilliseconds">Should output be in milliseconds or seconds?</param>
        /// <returns></returns>
        public static long ToUnixtime(this DateTime value, bool isInMilliseconds = true)
        {
            return isInMilliseconds
                ? (long)value.ToUniversalTime().Subtract(UNIXTIME_ZERO_POINT).TotalMilliseconds
                : (long)value.ToUniversalTime().Subtract(UNIXTIME_ZERO_POINT).TotalSeconds;
        }

        public static long ToUnixtime(this DateTime? value, bool isInMilliseconds = true)
        {
            if (!value.HasValue)
            {
                return 0;
            }

            return isInMilliseconds
                ? (long)value.Value.ToUniversalTime().Subtract(UNIXTIME_ZERO_POINT).TotalMilliseconds
                : (long)value.Value.ToUniversalTime().Subtract(UNIXTIME_ZERO_POINT).TotalSeconds;
        }
    }
}