using System.Text.Json;

namespace Backend.Models;

public class UserDailyQuizPacket{

    public int userID {get; set;}

    public int sleepQuality {get; set;}

    public bool disturbance {get;set;}

    public string? disturbanceDetails {get; set;}

    public DateTimeOffset sleepTime {get; set;}

    public DateTimeOffset wakeTime {get; set;}

}