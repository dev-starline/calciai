using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CalciAI.Persistance.Entities.Common
{
    public class ClientMaster : Table
    {

        [JsonPropertyName("clientMasterID")]
        public int ClientMasterID { get; set; }

        [JsonPropertyName("clientID")]
        public int ClientID { get; set; }

        [JsonPropertyName("clientName")]
        public string ClientName { get; set; }

        [JsonPropertyName("company")]
        public string Company { get; set; }

        [JsonPropertyName("city")]
        public string City { get; set; }

        [JsonPropertyName("mobile")]
        public string Mobile { get; set; }

        [JsonPropertyName("start_Date")]
        public DateTime Start_Date { get; set; }

        [JsonPropertyName("end_Date")]
        public DateTime End_Date { get; set; }

        [JsonPropertyName("status")]
        public bool Status { get; set; }

        [JsonPropertyName("created_By")]
        public string Created_By { get; set; }
    }
}
