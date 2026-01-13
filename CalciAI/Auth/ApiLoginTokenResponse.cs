using CalciAI.Models;
using System;
using System.Text.Json.Serialization;

namespace CalciAI.Auth
{
    public class ApiLoginTokenResponse : IModel
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }
    }
}
