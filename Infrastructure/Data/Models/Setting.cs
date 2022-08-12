namespace WeekOneApi.Infrastructure.Data.Models;
public class OtpSettings
{
    public string? Host { get; set; }
    public int Port { get; set; }
    public string? Password { get; set; }
    public string? Username { get; set; }
    public string? From { get; set; }
    public string? Token { get; set; }
}