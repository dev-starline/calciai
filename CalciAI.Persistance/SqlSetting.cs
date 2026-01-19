using CalciAI.Models;
using System.Text.Json.Serialization;

namespace CalciAI.Persistance
{
    public class SqlSetting : ISetting
    {
        //[JsonPropertyName("sAypUdyOe/IBrdX9sNY5OWD+zM+z7uvc95yQfvmZnzk=")] //connectionStrings
        [JsonPropertyName("WayjopuoLCNdpncV44RuKQS/SC4YLm3eNrx4lKV33zQ=")] //connectionStrings
        public string ConnectionString { get; set; }
    }
}
