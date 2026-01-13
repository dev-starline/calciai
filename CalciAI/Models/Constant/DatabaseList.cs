using System.Text.Json.Serialization;

namespace CalciAI.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DatabaseList
    {
        SkyBid = 0,
        MasterAdmin = 1
    }
}