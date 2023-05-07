using Backend.Models;
using Backend.Controllers;

namespace Backend.Helpers;

public class Optimizer : IOptimizer{
    
    private readonly IFitbitController _fitbitController;

    private readonly INestController _nestController;

    private readonly IDatabaseController _databaseController;

    public Optimizer(IFitbitController fitbitController, INestController nestController, IDatabaseController databaseController){
        _fitbitController = fitbitController;
        _nestController = nestController;
        _databaseController = databaseController;
    }

    public async Task optimizeTemp(User user){
        var heartBeatTask = _fitbitController.getHeartbeatData(user);
        var tempTask = _nestController.getTemperature(user);
        Console.WriteLine("Getting Sleep Session");
        var sessionTask = _databaseController.getSleepSession(user);
        Console.WriteLine("Got Sleep Session");
        var heartBeat = await heartBeatTask;
        var tempPacket = await tempTask;
        var temp = tempPacket.devices[0].traits.sdmdevicestraitsTemperature.ambientTemperatureCelsius;
        Console.WriteLine("Getting optimizing temperature");
        var optimalTemp = await getOptimalTemp(user, tempPacket.devices[0].traits.sdmdevicestraitsThermostatTemperatureSetpoint.heatCelsius);
        Console.WriteLine("Got optimized temperature");
        var now = DateTime.Now;
        var lag = DateTime.Now.TimeOfDay - TimeSpan.Parse(heartBeat.activitiesheartintraday.dataset[heartBeat.activitiesheartintraday.dataset.Count-1].time);

        //gets the most recent completed sleep session 
        var session = await sessionTask + 1;
        Console.WriteLine("Optimizing temperature");
        UserSleepStatePacket packet = new UserSleepStatePacket{
            userID = user.userID,
            sleepSession = session,
            logTime = DateTime.Now,
            heartBeat = heartBeat.activitiesheartintraday.dataset[heartBeat.activitiesheartintraday.dataset.Count-1].value,
            heartBeatLag = lag,
            currentTemp = temp,
            targetTemp = optimalTemp
        };

        await _databaseController.submitSleepPacket(packet);
        _nestController.setTemperature(user,optimalTemp, tempPacket.devices[0].name.Substring(tempPacket.devices[0].name.LastIndexOf("devices/")+8));
        Console.WriteLine("Optimized temperature");
    }

    private async Task<double> getOptimalTemp(User user, double currentTemp){
        return currentTemp;
    }

}