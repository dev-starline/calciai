using System.Text.Json.Serialization;

namespace CalciAI.Persistance.Entities
{
    public class FeedSecurities : Table
    {
        [JsonPropertyName("FeedSecurityID")]
        public int FeedSecurityID { get; set; }

        [JsonPropertyName("FeedID")]
        public int FeedID { get; set; }

        [JsonPropertyName("FeedName")]
        public string FeedName { get; set; }

        [JsonPropertyName("SecurityID")]
        public int SecurityID { get; set; }

        [JsonPropertyName("OperatorID")]
        public string OperatorID { get; set; }
    }
}
