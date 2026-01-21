using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CalciAI.Persistance.Entities.Common
{
    public class ProductMaster : Table
    {

        [JsonPropertyName("productID")]
        public int ProductID { get; set; }

        [JsonPropertyName("productName")]
        public string ProductName { get; set; }

        [JsonPropertyName("clientID")]
        public int ClientID { get; set; }
    }
}
