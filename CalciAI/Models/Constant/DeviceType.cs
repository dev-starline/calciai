using System.Text.Json.Serialization;

namespace CalciAI.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DeviceType
    {
        UNKNOWN, DESKTOP, MOBILE, ANDROID, IOS, ADMIN, DEALER, CLIENTWEB
    }
}
