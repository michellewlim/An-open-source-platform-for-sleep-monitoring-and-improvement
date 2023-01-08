using System.Text.Json;

namespace Backend.Models;

public class User{

    public int userID {get; set;}

    public int age {get; set;}

    public string? sex {get; set;}
    public FitbitAuth? fitbitData {get; set;}
    public string? nestID {get;set;}
    

}