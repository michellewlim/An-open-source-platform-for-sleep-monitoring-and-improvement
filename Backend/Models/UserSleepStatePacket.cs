using System.Text.Json;

namespace Backend.Models;

public class UserSleepStatePacket{

    public int userID {get;set;}

    public int sleepSession {get;set;}

    public DateTime logTime {get;set;}

    public int heartBeat {get;set;}

    public TimeSpan heartBeatLag {get;set;}

    public double currentTemp {get;set;}

    public double targetTemp {get;set;}

}