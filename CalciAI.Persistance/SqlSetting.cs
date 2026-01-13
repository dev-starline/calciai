using CalciAI.Models;
using System.Text.Json.Serialization;

namespace CalciAI.Persistance
{
    public class SqlSetting : ISetting
    {
        //[JsonPropertyName("sAypUdyOe/IBrdX9sNY5OWD+zM+z7uvc95yQfvmZnzk=")] //connectionStrings
        [JsonPropertyName("WayjopuoLCNdpncV44RuKSM6QZfJW2q0Xcu1VVuFPX4=")] //connectionStrings
        public string ConnectionString { get; set; }
    }
}
