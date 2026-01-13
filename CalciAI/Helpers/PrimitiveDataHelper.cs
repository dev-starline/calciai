using System;

namespace CalciAI.Helpers
{
    public static class PrimitiveDataHelper
    {
        public static DateTime? ParseDateString(string str)
        {
            return DateTime.TryParse(str, out DateTime dateTime) ? dateTime : null;
        }

        public static bool? ParseBoolString(string str)
        {
            return bool.TryParse(str, out bool value) ? value : null;
        }

        public static int? ParseIntString(string str)
        {
            return int.TryParse(str, out int value) ? value : null;
        }

        public static double? ParseDoubleString(string str)
        {
            return double.TryParse(str, out double value) ? value : null;
        }
    }
}
