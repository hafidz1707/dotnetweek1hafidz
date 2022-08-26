using WeekOneApi.Infrastructure.Data.Models;
namespace WeekOneApi.Infrastructure.Shared;
public class Response<T>
{
    public T? data {get; set;}
    public bool success {get; set;}
    public string? message {get; set;}
    public string? origin {get; set;}
}
public class ResponseNoData
{
    public bool success {get; set;}
    public string? message {get; set;}
    public string? origin {get; set;}
}

public class BadRequestResponse
{
    public string? error_code {get; set;}
    public string? message {get; set;}
}
// Responses For Service Registration
public class ResponseSRList
{
    public List<ServiceList>? vip {get; set;}
    public List<ServiceList>? booking {get; set;}
    public List<ServiceList>? walk_in {get; set;}
    public List<ServiceList>? progress {get; set;}
}
public class ResponseSRStatistic
{
    public int special_customer {get; set;}
    public int customer_less15_min {get; set;}
    public int customer_more15_min {get; set;}
    public int completed_customer {get; set;}
}
public class ResponseCircleCheck<T, U, V, W>
{
    public int id {get; set;}
    public int service_registration_id {get; set;}
    public string? service_advisor {get; set;}
    public DateTime service_date {get; set;}
    public string? dealer_service {get; set;}
    public string? customer_name {get; set;}
    public string? phone {get; set;}
    public string? vin {get; set;}
    public string? plate_number {get; set;}
    public string? vehicle_model {get; set;}
    public string? signature {get; set;}
    public T? exterior_view {get; set;}
    public U? tire_view {get; set;}
    public V? interior_view {get; set;}
    public W? complaint_notes_view {get; set;}

}