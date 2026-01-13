using System;
using System.Text.Json.Serialization;

namespace CalciAI.Persistance
{
    public interface ITable
    {
        int Created_By { get; set; }
        DateTime? Created_On { get; set; }
        int Updated_By { get; set; }
        DateTime? Updated_On { get; set; }
    }

    public abstract class Table : ITable
    {
        [JsonPropertyName("Created_By")]
        public int Created_By { get; set; }

        [JsonPropertyName("Created_On")]
        public DateTime? Created_On { get; set; }

        [JsonPropertyName("Updated_By")]
        public int Updated_By { get; set; }

        [JsonPropertyName("Updated_On")]
        public DateTime? Updated_On { get; set; }
    }
}