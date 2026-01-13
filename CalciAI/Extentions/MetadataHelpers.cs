using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;

namespace CalciAI.Extentions
{
    public static class MetadataHelpers
    {
        private const string DateFormat = "yyyy-MM-dd'T'HH:mm:ss";

        public static Dictionary<string, string> AddMetadata<T>(Dictionary<string, string> metadata, string key, T value)
        {
            if (value == null)
            {
                return metadata;
            }

            if (metadata == null)
            {
                metadata = new Dictionary<string, string>();
            }

            var strValue = SerializeValue(value);

            if (metadata.ContainsKey(key))
            {
                metadata[key] = strValue;
            }
            else
            {
                metadata.Add(key, strValue);
            }

            return metadata;
        }

        public static Dictionary<string, string> RemoveMetadata(Dictionary<string, string> metadata, string key)
        {
            if (metadata == null)
            {
                metadata = new Dictionary<string, string>();
            }

            if (metadata.ContainsKey(key))
            {
                metadata.Remove(key);
            }

            return metadata;
        }

        public static T GetMetadata<T>(Dictionary<string, string> metadata, string key)
        {
            if (metadata != null && metadata.ContainsKey(key))
            {
                return DeSerializeValue<T>(metadata[key]);
            }

            return default;
        }

        public static string SerializeValue<T>(T value)
        {
            var type = typeof(T);

            if (type.IsPrimitive ||
                    new Type[] { typeof(string), typeof(decimal), typeof(Guid) }.Contains(type) ||
                    type.IsEnum ||
                    Convert.GetTypeCode(type) != TypeCode.Object)
            {
                return value.ToString();
            }

            if (type == typeof(DateTime))
            {
                var dateTime = (DateTime)Convert.ChangeType(value, typeof(DateTime));
                return dateTime.ToUniversalTime().ToString(DateFormat, CultureInfo.InvariantCulture);
            }

            if (type == typeof(DateTimeOffset))
            {
                throw new InvalidOperationException("Not implemented");
            }

            if (type == typeof(TimeSpan))
            {
                throw new InvalidOperationException("Not implemented");
            }

            return JsonSerializer.Serialize(value);
        }

        public static T DeSerializeValue<T>(string strValue)
        {
            var type = typeof(T);

            if (type.IsPrimitive ||
                    new Type[] { typeof(string), typeof(decimal), typeof(Guid) }.Contains(type) ||
                    Convert.GetTypeCode(type) != TypeCode.Object)
            {
                return (T)Convert.ChangeType(strValue, typeof(T));
            }

            if (type.IsEnum)
            {
                var enumVal = Enum.Parse(type, strValue);
                return (T)Convert.ChangeType(enumVal, typeof(T));
            }

            if (type == typeof(DateTime))
            {
                var dateTime = DateTime.ParseExact(strValue, DateFormat, CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
                return (T)Convert.ChangeType(dateTime, typeof(T));
            }

            if (type == typeof(DateTimeOffset))
            {
                throw new InvalidOperationException("Not implemented");
            }

            if (type == typeof(TimeSpan))
            {
                throw new InvalidOperationException("Not implemented");
            }

            return JsonSerializer.Deserialize<T>(strValue);
        }
    }
}
