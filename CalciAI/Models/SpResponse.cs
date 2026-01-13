using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CalciAI.Models
{
    public class SpResponse : IModel
    {
        [JsonPropertyName("returnValue")]
        public Dictionary<string, string> ReturnValue { get; set; } = new Dictionary<string, string>();
    }
}