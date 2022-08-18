namespace WeekOneApi.Infrastructure.Data.Models
{
    public class User
    {
        public int id { get; set; }
        public string? username { get; set; }
        public string? name { get; set; }
        public string? password { get; set; }
        public string? dealer_code {get; set; }
        public string? dealer_name { get; set; }
        public string? email { get; set; }
        public string? phone {get; set; }
        public string? position_code { get; set; }
        public string? position_name {get; set; }
        public bool is_registered { get; set; }
        public int? pin_otp { get; set; }
        public string? DeviceId { get; set; }
        public string? Version {get; set; }
    }
    public class UserChanger
    {
        public int id { get; set; }
        public string? email { get; set; }
        public string? phone {get; set; }
        public string? password { get; set; }
    }
}