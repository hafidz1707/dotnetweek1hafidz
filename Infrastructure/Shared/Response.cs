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
