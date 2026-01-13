using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CalciAI.Models.Admin
{

    public class DomainMasterModel : IModel
    {
        [JsonPropertyName("domainID")]
        public int DomainID { get; set; }

        [JsonPropertyName("domainName")]
        public string DomainName { get; set; }

        [JsonPropertyName("url")]
        public string URL { get; set; }

        [JsonPropertyName("fetch_Type")]
        public string Fetch_Type { get; set; }

        [JsonPropertyName("target_Point")]
        public string? Target_Point { get; set; }

        [JsonPropertyName("target_Mode")]
        public string? Target_Mode { get; set; }

       
    }
}
