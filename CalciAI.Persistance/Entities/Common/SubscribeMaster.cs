using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CalciAI.Persistance.Entities.Common
{
    public class SubscribeMaster : Table
    {

        [JsonPropertyName("subscribeID")]
        public int SubscribeID { get; set; }

        [JsonPropertyName("cityID")]
        public int CityID { get; set; }

        [JsonPropertyName("cityName")]
        public string CityName { get; set; }

        [JsonPropertyName("productID")]
        public int ProductID { get; set; }

        [JsonPropertyName("productName")]
        public string ProductName { get; set; }

        [JsonPropertyName("urlID")]
        public int URLID { get; set; }

        [JsonPropertyName("urlName")]
        public string URLName { get; set; }

        [JsonPropertyName("selectedProduct")]
        public string SelectedProduct { get; set; }

        [JsonPropertyName("gst")]
        public bool GST { get; set; }
    }
}
