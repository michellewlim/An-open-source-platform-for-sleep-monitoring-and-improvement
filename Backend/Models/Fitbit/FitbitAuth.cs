namespace Backend.Models.Fitbit;

public class FitbitAuth{
    
    public string? fitbitID {get; set;}

    public string? accessToken {get; set;}

    public string? refreshToken {get; set;}

    public DateTime? expires {get; set;}
}