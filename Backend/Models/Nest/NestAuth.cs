namespace Backend.Models.Nest;

public class NestAuth{

    public string? nestID {get; set;}

    public string? accessToken {get; set;}

    public string? refreshToken {get; set;}

    public DateTime? expires {get; set;}
}