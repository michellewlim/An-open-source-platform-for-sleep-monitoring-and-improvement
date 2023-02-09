namespace Backend.Models.Fitbit;
public class FitbitOnboardPacket{
    
    public int userID {get; set;}
    
    public string? fitbitID {get; set;}

    public string? accessToken {get; set;}

    public string? refreshToken {get; set;}

}