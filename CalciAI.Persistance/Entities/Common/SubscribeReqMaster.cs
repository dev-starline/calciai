using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CalciAI.Persistance.Entities.Common
{
    public class SubscribeReqMaster : Table
    {

        [JsonPropertyName("sRequestID")]
        public int SRequestID { get; set; }

        [JsonPropertyName("urlName")]
        public string URLName { get; set; }
    }
}
