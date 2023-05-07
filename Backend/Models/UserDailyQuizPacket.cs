using System.Text.Json;

namespace Backend.Models;

public class UserDailyQuizPacket{

    public int userID {get; set;}

    public int sleepSession {get; set;}

    public int q1 {get; set;}

    public int q2 {get; set;}

    public int q3 {get; set;}

    public int q4 {get; set;}

    public int q5 {get; set;}

    public int q6 {get; set;}

    public int q7 {get; set;}
    
    public DateTimeOffset wakeTime {get; set;}

}