namespace WeekOneApi.Infrastructure.Data.Models;
public class ServiceList
{
    public int id {get; set;}
    public string? reg_number {get; set;}
    public string? queue_number {get; set;}
    public string? service_advisor {get; set;}
    public string? name {get; set;}
    public string? plate_number {get; set;}
    public int input_source_id {get; set;}
    public string? input_source {get; set;}
    public string? preferred_sa {get; set;}
    public string? estimated_service {get; set;}
    public TimeSpan? waiting_time {get; set;}
    public int status_id {get; set;} = 1;
    public string? status {get; set;} = "Waiting For Service";
    public string? is_vip {get; set;} = "-";
    public string? is_rework {get; set;} = "-";
    public string? is_ontime {get; set;} = "-";
    public DateTime booking_date_time {get; set;}
    public DateTime create_time {get; set;}
}