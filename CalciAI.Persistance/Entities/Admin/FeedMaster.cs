using CalciAI.Models;
using System.Text.Json.Serialization;

namespace CalciAI.Persistance.Entities
{
    public class FeedMaster : Table
    {
        [JsonPropertyName("FeedID")]
        public int FeedID { get; set; }

        [JsonPropertyName("OperatorID")]
        public string OperatorID { get; set; }

        [JsonPropertyName("FeedCode")]
        public string FeedCode { get; set; }

        [JsonPropertyName("FeedName")]
        public string FeedName { get; set; }

        [JsonPropertyName("FeedModule")]
        public string FeedModule { get; set; }

        [JsonPropertyName("FeedModuleDisplayName")]
        public string FeedModuleDisplayName { get; set; }

        [JsonPropertyName("FeedType")]
        public FeedTypes FeedType { get; set; }

        [JsonPropertyName("FeedServer")]
        public string FeedServer { get; set; }

        [JsonPropertyName("FeedUserName")]
        public string FeedUserName { get; set; }

        [JsonPropertyName("FeedPassword")]
        public string FeedPassword { get; set; }

        [JsonPropertyName("FeedConnectionStatus")]
        public bool FeedConnectionStatus { get; set; }

        [JsonPropertyName("FeedDescription")]
        public string FeedDescription { get; set; }

        [JsonPropertyName("FeedStatus")]
        public bool FeedStatus { get; set; }

        [JsonPropertyName("SecurityID")]
        public string SecurityID { get; set; }

        [JsonPropertyName("SecurityName")]
        public string SecurityName { get; set; }

        [JsonPropertyName("MasterSymbolID")]
        public int MasterSymbolID { get; set; }
    }
}
