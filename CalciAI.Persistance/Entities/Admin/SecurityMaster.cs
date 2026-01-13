using CalciAI.Models;
using System.Text.Json.Serialization;

namespace CalciAI.Persistance.Entities
{
    public class SecurityMaster : Table
    {
        [JsonPropertyName("SecurityID")]
        public int SecurityID { get; set; }

        [JsonPropertyName("OperatorID")]
        public string OperatorID { get; set; }

        [JsonPropertyName("SecurityCode")]
        public string SecurityCode { get; set; }

        [JsonPropertyName("SecurityName")]
        public string SecurityName { get; set; }

        [JsonPropertyName("SecurityLimitPassby")]
        public SecurityLimitPassBy SecurityLimitPassby { get; set; }

        [JsonPropertyName("SecurityPendingLimitHighLow")]
        public bool SecurityPendingLimitHighLow { get; set; }

        [JsonPropertyName("SecurityWeeklyRollover")]
        public bool SecurityWeeklyRollover { get; set; }

        [JsonPropertyName("SecurityGTC")]
        public SecurityGTC SecurityGTC { get; set; }

        [JsonPropertyName("SecurityDescription")]
        public string SecurityDescription { get; set; }

        [JsonPropertyName("SecurityStatus")]
        public bool SecurityStatus { get; set; }

        [JsonPropertyName("MasterSymbolCount")]
        public int MasterSymbolCount { get; set; }
    }
}
