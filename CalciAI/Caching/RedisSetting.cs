using CalciAI.Models;
using System.Text.Json.Serialization;

namespace CalciAI.Caching
{
    public class RedisSetting : ISetting
    {
        [JsonPropertyName("eSJWrUesb1J2iT8PVUU1sw==")] // endPoints
        public string EndPoints { get; set; }

        [JsonPropertyName("y7d8e4kbyIVrz+bV80tpzA==")] // defaultDatabase
        public int DefaultDatabase { get; set; }

        [JsonPropertyName("5+EL9lpH6WVhfQPsGwXOSQ==")] // password
        public string Password { get; set; }

        [JsonPropertyName("LRvC/h8lrsHOGp09YuHgQXaLVVbaIAEVxOEKW9oPJGk=")] // abortOnConnectFail
        public bool AbortOnConnectFail { get; set; }
    }
}
