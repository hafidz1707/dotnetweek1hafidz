using System.ComponentModel.DataAnnotations;

namespace WeekOneApi.Infrastructure.Data.Models
{
    public class Request<T>
    {
        public T? data {get; set;}
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
    public class RegisterWalkIn
    {
        public string? plate_number {get; set;} 
        //[DataType(DataType.Date)]
        public DateTime time_stamp {get; set;}
    }
    public class RegisterBooking
    {
        public string? plate_number {get; set;} 
        //[DataType(DataType.Date)]
        public DateTime time_stamp {get; set;}
        public string? name {get; set;}
        public string? is_vip {get; set;}
    }
    public class UpdateServiceStatus
    {
        public int service_registration_id {get; set;} 
        public int service_status {get; set;}
    }
}