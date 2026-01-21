using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CalciAI.Models.client
{

    public class DashboardModel : IModel
    {
        [JsonPropertyName("cityCnt")]
        public int CityCnt { get; set; }

        [JsonPropertyName("productCnt")]
        public int ProductCnt { get; set; }

        [JsonPropertyName("subscribeCnt")]
        public int SubscribeCnt { get; set; }

        [JsonPropertyName("officeUserCnt")]
        public int OfficeUserCnt { get; set; }

        [JsonPropertyName("clientID")]
        public int ClientID { get; set; }

    }
}
