using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CalciAI.Models.Admin
{
    public class ResetUserPasswordModel : IModel
    {
        [JsonPropertyName("userName")]
        public string UserName { get; set; }

        //[JsonPropertyName("userMasterId")]
        //public int UserMasterID { get; set; }

        [JsonPropertyName("oldUserPassword")]
        public string OldUserPassword { get; set; }

        [JsonPropertyName("userPassword")]
        public string UserPassword { get; set; }

        [JsonPropertyName("userConfirmPassword")]
        public string UserConfirmPassword { get; set; }
    }
}
