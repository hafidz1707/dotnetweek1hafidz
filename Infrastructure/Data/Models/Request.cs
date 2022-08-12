namespace WeekOneApi.Infrastructure.Data.Models
{
    public class Request
    {
        public User? Data {get; set;}
    }

    public class RequestId
    {
        public int id {get; set;}
    }

    public class ChangePassword
    {
        public string? current_password {get; set;}
        public string? new_password {get; set;}
        public string? new_password_confirmation {get; set;}
    }
    public class OtpRequest
    {
        public int? id { get; set; }
        public int? pin_otp {get; set;}
    }
}