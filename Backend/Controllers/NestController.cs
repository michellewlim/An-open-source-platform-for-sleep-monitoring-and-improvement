using Backend.Models;
using Backend.Models.Nest;
using Backend.Helpers;
using System.Text.Json;

namespace Backend.Controllers;

public class NestController : INestController{

    private readonly IConfiguration _config;

    private readonly INestAuthenticator _nestAuthenticator;

    readonly IDatabaseController _databaseController;

    public NestController(IConfiguration configuration, IDatabaseController databaseController, INestAuthenticator nestAuthenticator){;
        _databaseController = databaseController;
        _config = configuration;
        _nestAuthenticator = nestAuthenticator;
    }

    public async Task<NestDevicesPacket> getTemperature(User user){
        await _nestAuthenticator.checkAuth(user);
        
        var client = new HttpClient();
        var projectID = $"{_config["Nest:Project ID"]}";
        var request = new HttpRequestMessage(HttpMethod.Get, $"https://smartdevicemanagement.googleapis.com/v1/enterprises/{projectID}/devices/");
        request.Headers.Add("Authorization", $"Bearer {user.nestData.accessToken}");
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        //Console.WriteLine(await response.Content.ReadAsStringAsync());
        var nestDevicesPacket = JsonSerializer.Deserialize<NestDevicesPacket>(await response.Content.ReadAsStringAsync());
        if(nestDevicesPacket == null){
            throw(new NullReferenceException("nestDevicesPacket is null"));
        }
        Console.WriteLine($"Ambient Temp for user {user.userID} is {nestDevicesPacket.devices[0].traits.sdmdevicestraitsTemperature.ambientTemperatureCelsius}");
        return nestDevicesPacket;
    }

    public async Task setTemperature(User user, double temperature, string deviceID){
        await _nestAuthenticator.checkAuth(user);

        var projectID = $"{_config["Nest:Project ID"]}";

        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Post, $"https://smartdevicemanagement.googleapis.com/v1/enterprises/{projectID}/devices/{deviceID}:executeCommand");
        request.Headers.Add("Authorization", $"Bearer {user.nestData.accessToken}");
        var content = new StringContent("{\r\n    \"command\": \"sdm.devices.commands.ThermostatTemperatureSetpoint.SetHeat\",\r\n    \"params\": {\r\n        \"heatCelsius\": " + $"{temperature}" + "\r\n    }\r\n}", null, "application/json");
        request.Content = content;
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        Console.WriteLine($"Temperature set to {temperature} in celcius or {temperature * 9/5 + 32} in fahrenheit for user {user.userID}");
        return;
    }

}