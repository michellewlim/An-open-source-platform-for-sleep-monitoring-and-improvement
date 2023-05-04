namespace Backend.Models.Nest;
public class NestOnboardPacket{
    
    public int userID {get; set;}
    
    public string? accessToken {get; set;}

    public string? refreshToken {get; set;}

    public int expires_in {get; set;}
}