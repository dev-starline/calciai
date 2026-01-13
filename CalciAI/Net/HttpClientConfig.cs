using CalciAI.Models;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CalciAI.Net
{
    public class HttpClientConfig : ISetting
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("baseUrl")]
        public string BaseUrl { get; set; }

        [JsonPropertyName("authRequired")]
        public bool AuthRequired { get; set; }

        [JsonPropertyName("timeout")]
        public int TimeOut { get; set; }

        [JsonPropertyName("baseHeaders")]
        public Dictionary<string, string> BaseHeaders { get; set; }

        [JsonPropertyName("extraParams")]
        public Dictionary<string, object> ExtraParams { get; set; }

        public override string ToString()
        {
            return $"Name:{Name}, BaseUrl:{BaseUrl}";
        }
    }

    public class HttpClientConfigWithEncrypt : ISetting
    {
        [JsonPropertyName("ZoMRgY+y6GWcr3uXdXOmIg==")]
        public string Name { get; set; }

        [JsonPropertyName("c23wRrtqOv6v5CJGsuRT4Q==")]
        public string BaseUrl { get; set; }

        [JsonPropertyName("0N7Bt3f22bBBEQOmFdBIwg==")]
        public bool AuthRequired { get; set; }

        [JsonPropertyName("DJynaJ84wzaw5/pZD0ElOQ==")]
        public int TimeOut { get; set; }

        [JsonPropertyName("AyG3lTBdrkR2nqFmfgWPaQ==")]
        public Dictionary<string, string> BaseHeaders { get; set; }

        [JsonPropertyName("oXtaAC+qxaYUHfrbwjEEfw==")]
        public Dictionary<string, object> ExtraParams { get; set; }

        public override string ToString()
        {
            return $"Name:{Name}, BaseUrl:{BaseUrl}";
        }
    }
}
