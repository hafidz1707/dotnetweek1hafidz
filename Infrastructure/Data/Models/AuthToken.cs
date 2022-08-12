namespace WeekOneApi.Infrastructure.Data.Models;

public class AuthToken
{
    public int id {get; set;}
    public int userId {get; set;}
    public string? token {get; set;}
    public string? role {get; set;}
    public int expiredAt {get; set;}
}