using System.Text.Json;
using System.Text.Json.Serialization;

namespace CalciAI.Serialization
{
    public static class SerializationExtension
    {
        public static JsonSerializerOptions DefaultOptions
        {
            get
            {
                var options = new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                };

                return options;
            }
        }
    }
}
