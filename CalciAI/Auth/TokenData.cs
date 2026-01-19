using CalciAI.Models;
using System;
using System.Text.Json.Serialization;

namespace CalciAI.Auth
{
    public class TokenData : IModel
    {
        [JsonPropertyName("sub")]
        public string Username { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; }

        //[JsonPropertyName("iss")]
        //public string Operator { get; set; }

        [JsonPropertyName("exp")]
        public DateTime? Expiration { get; set; }

        [JsonPropertyName("ip")]
        public string IpAddress { get; set; }


        //[JsonPropertyName("isreadonlypassword")]
        //public bool IsReadonlyPassword { get; set; }

        //[JsonPropertyName("intime")]
        //public string InTime { get; set; }

        //[JsonPropertyName("serverIP")]
        //public string ServerIP { get; set; }
    }
}