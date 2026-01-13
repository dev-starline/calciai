using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CalciAI.Models
{
    public interface IModel
    {
    }

    public class ResultModel : IModel
    {
        public ResultModel(bool isSuccess)
        {
            Success = isSuccess;
        }

        public bool Success { get; set; }
    }

    public class ModelList<T> : List<T>, IModel
    {
        public ModelList() { }

        public ModelList(IEnumerable<T> data) : base(data)
        {

        }
    }

    public interface IException { }

    public class UpsertInfo : IModel
    {
        [JsonPropertyName("createdBy")]
        public int Created_By { get; set; } = 1;

        [JsonPropertyName("createdOn")]
        public DateTime Created_On { get; set; } = DateTime.Now;

        [JsonPropertyName("updatedBy")]
        public int Updated_By { get; set; } = 1;

        [JsonPropertyName("updatedOn")]
        public DateTime Updated_On { get; set; } = DateTime.Now;
    }
}
