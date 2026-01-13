using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CalciAI.Models.client
{

    public class ProductMasterModel : IModel
    {
        [JsonPropertyName("productID")]
        public int ProductID { get; set; }

        [JsonPropertyName("productName")]
        public string ProductName { get; set; }
       
    }
}
