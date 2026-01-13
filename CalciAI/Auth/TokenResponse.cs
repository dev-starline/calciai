using CalciAI.Models;
using System;
using System.Text.Json.Serialization;

namespace CalciAI.Auth
{
    public class TokenResponse : IModel
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("expiration")]
        public DateTime? Expiration { get; set; }
    }
}
