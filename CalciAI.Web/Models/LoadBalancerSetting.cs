using System.Text.Json.Serialization;

namespace CalciAI.Web.Models
{
    public class LoadBalancerSetting
    {
        public const string SectionName = "LoadBalancer";

        [JsonPropertyName("CIDR")]
        public string Cidr { get; set; }

        [JsonPropertyName("Addresses")]
        public string[] Addresses { get; set; }
    }
}
