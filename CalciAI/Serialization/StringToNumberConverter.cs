using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CalciAI.Serialization
{
    public class StringToNumberConverter : JsonConverter<object>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            // see https://stackoverflow.com/questions/1749966/c-sharp-how-to-determine-whether-a-type-is-a-number
            return Type.GetTypeCode(typeToConvert) switch
            {
                TypeCode.Byte or TypeCode.SByte or TypeCode.UInt16 or TypeCode.UInt32 or TypeCode.UInt64 or TypeCode.Int16 or TypeCode.Int32 or TypeCode.Int64 or TypeCode.Decimal or TypeCode.Double or TypeCode.Single => true,
                _ => false,
            };
        }
        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var s = reader.GetString();

                if(typeToConvert == typeof(int) && int.TryParse(s, out var i))
                {
                    return i;
                }

                if(typeToConvert == typeof(double) && double.TryParse(s, out var d))
                {
                    return d;
                }

                throw new InvalidCastException($"unable to parse {s} to number");
            }

            if (reader.TokenType == JsonTokenType.Number)
            {
                return reader.TryGetInt64(out long l) ? l : reader.GetDouble();
            }

            using JsonDocument document = JsonDocument.ParseValue(ref reader);
            throw new InvalidCastException($"unable to parse {document.RootElement} to number");
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            var str = value.ToString();  // I don't want to write int/double/double/...  for each case, so I just convert it to string . You might want to replace it with strong type version.
            if (int.TryParse(str, out var i))
            {
                writer.WriteNumberValue(i);
            }
            else if (double.TryParse(str, out var d))
            {
                writer.WriteNumberValue(d);
            }
            else
            {
                throw new InvalidCastException($"unable to parse {str} to number");
            }
        }
    }
}
