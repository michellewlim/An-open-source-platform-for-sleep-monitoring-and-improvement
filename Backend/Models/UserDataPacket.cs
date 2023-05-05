using System.Text.Json;

namespace Backend.Models;

public class UserDataPacket{
    
    public int userID {get; set;}

    public int age {get; set;}

    public string? sex {get; set;}
}