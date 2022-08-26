namespace WeekOneApi.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
public class CircleCheck
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
    public List<ExteriorView>? exterior_view {get; set;}
    public TireView? tire_view {get; set;}
    public InteriorView? interior_view {get; set;}
    public ComplaintView? complaint_notes_view {get; set;}
}
public class ExteriorView
{
    public int id {get; set;}
    public int circle_check_header_id {get; set;}
    public int service_registration_id {get; set;}
    public int data_type {get; set;}
    public string? data_type_text {get; set;}
    public int vehicle_condition {get; set;}
    public string? notes {get; set;}
    public string? image_path {get; set;}
}
public class TireView
{
    public int id {get; set;}
    public int circle_check_header_id {get; set;}
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
public class InteriorView
{
    public int id {get; set;}
    public int circle_check_header_id {get; set;}
    public int service_registration_id {get; set;}
    public int stnk {get; set;}
    public int service_booklet {get; set;}
    public int spare_tire {get; set;}
    public int safety_kit {get; set;}
    public int fuel_gauge {get; set;}
    public int other_stuff {get; set;}
    public string? other_stuff_notes {get; set;}
    public string? interior_photo_1 {get; set;}
    public string? interior_photo_2 {get; set;}
    public string? interior_photo_3 {get; set;}
}
public class ComplaintView
{
    public int id {get; set;}
    public int circle_check_header_id {get; set;}
    public int service_registration_id {get; set;}
    public string? notes {get; set;}
}