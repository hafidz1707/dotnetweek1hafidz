namespace WeekOneApi.Infrastructure.Data.Models
{
    public class Data
    {
        public int id { get; set; }
        public bool is_registered { get; set; }
        public string? change { get; set; }
        public string? supervisor_code { get; set; }
        public Profile? profile { get; set; }
        public Dealer? dealer { get; set; }
        public Analytic_user? analytic_user { get; set; }
        public Access_token? access_token { get; set; }
        public string? force_update {get; set; }
        public string? is_data_match { get; set; }
        public string? go_live { get; set; }
    }
    public class Profile
    {
        public int trainee_id {get; set;}
        public string? trainee_phone {get; set;} 
        public string? trainee_email {get; set;}
        public string? change {get; set;} 
        public string? name {get; set;}
        public string? email {get; set;} 
        public string? phone {get; set;} 
        public string? position {get; set;}
        public string? level {get; set;} 
        public string? photo {get; set;} 
    }
    public class Dealer {
        public int id {get; set;}
        public string? name {get; set;} 
        public string? code {get; set;}
        public string? city {get; set;} 
        public string? dealer {get; set;}
        public string? group {get; set;} 
    }
    public class Analytic_user {
        public int dealer_id {get; set;}
        public string? dealer_branch_code {get; set;} 
        public string? job_position {get; set;}
        public string? city {get; set;} 
    }
    public class Access_token {
        public string? auth_token {get; set;} 
        public string? type {get; set;}
        public int expires_at {get; set;} 
    }
}