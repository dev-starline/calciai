using System.Text.Json.Serialization;

namespace CalciAI.Models
{
    public class Page
    {
        [JsonPropertyName("offset")]
        public int Offset { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }
    }
}
