using System.Text.Json.Serialization;

namespace CalciAI.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum UserRole
    {
        S2S = 0,
        MasterAdmin = 1,
        Admin = 2,
        Dealer = 3,
        Client = 4,
        AdminOfficeUser = 5,
        DealerOfficeUser = 6,
        Operator = 7,
        ClientWeb = 8,
        ComboDealer = 9
    }
}