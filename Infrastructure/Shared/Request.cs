using System.ComponentModel.DataAnnotations;

namespace WeekOneApi.Infrastructure.Shared;
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
// Requests For Service Registration
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

// Request for Circle-Check
public class SaveCircleCheck
{
    public int service_registration_id {get; set;}
    public CustomerInfo? customer_info {get; set;}
    public Interior interior {get; set;}
    public string? complaint_notes {get; set;} 
}
public class CustomerInfo 
{
    public string? customer_name {get; set;}
    public string? vin {get; set;}
    public string? phone {get; set;}
}
public class Interior
{
    public int stnk {get; set;}
    public int service_booklet {get; set;}
    public int spare_tire {get; set;}
    public int safety_kit {get; set;}
    public int fuel_gauge {get; set;}
    public int other_stuff {get; set;}
    public string? other_stuff_notes {get; set;}
}
// Save Interior View Photo
public class SaveInteriorViewPhoto
{
    public int service_registration_id {get; set;}
    public string? interior_photo_1 {get; set;}
    public string? interior_photo_2 {get; set;}
    public string? interior_photo_3 {get; set;}
}
// Create Update Exterior View
public class CreateUpdateExteriorView
{
    public int service_registration_id {get; set;}
    public int type {get; set;}
    public int vehicle_condition {get; set;}
    public string? notes {get; set;}
    public string? image_path {get; set;}
}
// Create Update Tire View
public class CreateUpdateTireView
{
    public int service_registration_id {get; set;}
    public string? front_right {get; set;}
    public string? front_right_photo_1 {get; set;}
    public string? front_right_photo_2 {get; set;}
    public string? front_right_photo_3 {get; set;}
    public string? front_left {get; set;}
    public string? front_left_photo_1 {get; set;}
    public string? front_left_photo_2 {get; set;}
    public string? front_left_photo_3 {get; set;}
    public string? back_right {get; set;}
    public string? back_right_photo_1 {get; set;}
    public string? back_right_photo_2 {get; set;}
    public string? back_right_photo_3 {get; set;}
    public string? back_left {get; set;}
    public string? back_left_photo_1 {get; set;}
    public string? back_left_photo_2 {get; set;}
    public string? back_left_photo_3 {get; set;}
}
// Finalize Circle Check
public class FinalizeCircleCheck
{
    public int service_registration_id {get; set;}
    public string? signature {get; set;}
    public string? generated_file {get; set;}
}