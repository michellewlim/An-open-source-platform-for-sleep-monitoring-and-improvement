using System.Text.Json;

namespace Backend.Models;

public class TempDataPacket{

    public int userID {get; set;}
    public string? nestID {get; set;}
    public int temp {get; set;}
    public DateTime timeStamp {get; set;}
}