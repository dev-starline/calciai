using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CalciAI.Models.Admin
{
    public class UserMasterModel : IModel
    {
        [JsonPropertyName("userMasterId")]
        public int UserMasterID { get; set; }

      

        [JsonPropertyName("userName")]
        public string UserName { get; set; }

       

        [JsonPropertyName("userRole")]
        public string UserRole { get; set; }

        [JsonPropertyName("userPassword")]
        public string UserPassword { get; set; }

        [JsonPropertyName("lastLoginIp")]
        public string LastLoginIP { get; set; }

        [JsonPropertyName("lastLogintime")]
        public DateTime? LastLoginTime { get; set; }

        [JsonPropertyName("userStatus")]
        public bool UserStatus { get; set; }

        [JsonPropertyName("createdOn")]
        public DateTime? Created_On { get; set; }
    }
}
