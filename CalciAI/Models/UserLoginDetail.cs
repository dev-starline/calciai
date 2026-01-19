using System.Text.Json.Serialization;

namespace CalciAI.Models
{
    public class UserLoginDetail : IModel
    {
        [JsonPropertyName("UserName")]
        public string UserName { get; set; }

        [JsonPropertyName("Name")]
        public string Name { get; set; }

        [JsonPropertyName("UserRole")]
        public string UserRole { get; set; }

        [JsonPropertyName("IsSuccess")]
        public bool IsSuccess { get; set; }

        [JsonPropertyName("Msg")]
        public string Msg { get; set; }

        [JsonPropertyName("IsReadonlyPassword")]
        public bool IsReadonlyPassword { get; set; }

        [JsonPropertyName("Parents")]
        public string Parents { get; set; }

        [JsonPropertyName("ServerIP")]
        public string ServerIP { get; set; }

        [JsonPropertyName("enforcePassword")]
        public bool EnforcePassword { get; set; }
    }
}
