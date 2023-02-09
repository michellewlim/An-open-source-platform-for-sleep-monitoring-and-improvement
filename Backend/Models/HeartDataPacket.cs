using System;
using System.Collections.Generic;

using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;
namespace Backend.Models;

public class HeartDataPacket{

    public int userID {get; set;}
    public string? fitbitID {get; set;}

}