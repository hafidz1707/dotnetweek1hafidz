namespace WeekOneApi.Infrastructure.Data.Models
{
    public class Response
    {
        public Data? data {get; set;}
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
}