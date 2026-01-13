using System.Text.Json.Serialization;

namespace CalciAI.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum FeedModules
    {
        MT5ManagerFeeder,
        MT5ClientFeeder
    }
}