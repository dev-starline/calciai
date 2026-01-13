using System.Text.Json.Serialization;

namespace CalciAI.Models
{
    public class DeviceDetail
    {
        [JsonPropertyName("clientIP")]
        public string ClientIP { get; set; }

        [JsonPropertyName("device")]
        public string Device { get; set; }

        [JsonPropertyName("reason")]
        public string Reason { get; set; }

    }
}