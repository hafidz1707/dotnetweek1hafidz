namespace WeekOneApi.Infrastructure.Data.Models
{
    public class UserDTO
    {
        public int id { get; set; }
        public string? name { get; set; }
        public string? username { get; set; }
        public string? dealer_code { get; set; }
        public string? dealer_name { get; set; }
        public string? email { get; set; }
        public string? phone { get; set; }
        public string? position_code { get; set; }
        public string? position_name { get; set; }
        public UserDTO() { }
        public UserDTO(User userItem) =>
        (id, name, username, dealer_code, dealer_name, email, phone, position_code, position_name) 
        = (userItem.id, userItem.name, userItem.username, userItem.dealer_code, userItem.dealer_name, userItem.email, userItem.phone, userItem.position_code, userItem.position_name);
    }
}
