using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CalciAI.Models
{
    /// <summary>
    /// We can use currency parameter from exchange for launching game and modifying balance response for all providers. 
    /// This will take lot of time but need to capture this for dynamic currency setup. {OP}:username:{CUR}. 
    /// So our code would be parsing three tokens and take action appropriately.
    /// </summary>
    public class OperatorUserId : IModel
    {
        private string _operatorId;

        //[JsonPropertyName("operatorId")]
        //public string OperatorId { get { return _operatorId?.ToUpper(); } set { _operatorId = value; } }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("userId")]
        //public string UserId => $"{OperatorId}:{Username}";
        public string UserId => $"{Username}";

        public OperatorUserId()
        {

        }

        public OperatorUserId(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return;
            }

            var parts = code.Split(":");

            if (parts.Length < 2)
            {
                return;
            }

            //OperatorId = parts[0];
            Username = parts[1];
        }

        public OperatorUserId(string operatorId, string username)
        {
            //OperatorId = operatorId;
            Username = username;
        }

        public bool IsNull()
        {
            return string.IsNullOrEmpty(_operatorId);
        }

        public bool IsValid()
        {
            //return string.IsNullOrEmpty(OperatorId) || string.IsNullOrEmpty(Username);
            return  string.IsNullOrEmpty(Username);
        }

        public static implicit operator OperatorUserId(string value)
        {
            return new OperatorUserId(value);
        }

        public static implicit operator string(OperatorUserId value) => value.ToString();

        public override bool Equals(object obj)
        {
            if (obj is string or OperatorUserId)
            {
                return GetHashCode() == obj.GetHashCode();
            }

            return false;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            // return $"{OperatorId}:{Username}";
            return $"{Username}";
        }
    }

    public class OuWithMasterPassword : OperatorUserId
    {
        [JsonPropertyName("masterPassword")]
        public string MasterPassword { get; set; }
    }
}
