namespace WeekOneApi.Infrastructure.Shared;
public enum SRStatus
{
    Waiting_For_Service = 1,
    SA_Checking = 2,
    In_Progress_Service = 3,
    Completed = 4,
    Canceled = 5
};