using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CalciAI.Models.client
{

    public class CityMasterModel : IModel
    {
        [JsonPropertyName("cityID")]
        public int CityID { get; set; }

        [JsonPropertyName("cityName")]
        public string CityName { get; set; }

        [JsonPropertyName("clientID")]
        public int ClientID { get; set; }

    }
}
