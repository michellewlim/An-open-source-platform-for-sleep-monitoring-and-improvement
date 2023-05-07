using System.Text.Json;

using Backend.Models.Fitbit;
using Backend.Models.Nest;
namespace Backend.Models;

public class User{
    public int userID {get; set;}
    public int age {get; set;}
    public string? sex {get; set;}
    public FitbitAuth? fitbitData {get; set;}
    public NestAuth? nestData {get;set;}
    public int? minutesSleeping {get; set;}
}