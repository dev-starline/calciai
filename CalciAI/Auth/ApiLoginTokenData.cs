using CalciAI.Models;
using System;
using System.Text.Json.Serialization;

namespace CalciAI.Auth
{
    public class ApiLoginTokenData : IModel
    {
        [JsonPropertyName("sub")]
        public string Username { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("iss")]
        public string Operator { get; set; }

        [JsonPropertyName("ip")]
        public string IpAddress { get; set; }
    }
}