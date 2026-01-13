using System.Text.Json.Serialization;

namespace CalciAI.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Modules
    {
        Admin,
        Client
    }
}