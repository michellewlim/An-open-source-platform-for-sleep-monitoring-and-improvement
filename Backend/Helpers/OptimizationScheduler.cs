using Backend.Models;
using Backend.Controllers;
namespace Backend.Helpers;

public class OptimizationScheduler : IOptimizationScheduler{
    
    private readonly IFitbitController _fitbitController;

    private readonly INestController _nestController;

    private List<User> sleepingUsers {get;set;}

    private Boolean optimizing {get;set;}

    public OptimizationScheduler(IFitbitController fitbitController, INestController nestController){
        _fitbitController = fitbitController;
        _nestController = nestController;
        sleepingUsers = new List<User>();
        //optimizeUserSleep();
    }

    public async Task<int> sleepUser(User user){
        var userToAdd = sleepingUsers.Find((sleepingUser) => sleepingUser.userID == user.userID);
        if(userToAdd is not null){
            return 0;
        }
        sleepingUsers.Add(user);
        Console.WriteLine("User Slept: " + user.userID);
        if(!optimizing){
            optimizeUserSleep();
        }
        return 1;
    }

    public int wakeUser(User user){
        var userToRemove = sleepingUsers.Find((sleepingUser) => sleepingUser.userID == user.userID);
        if(userToRemove is null){
            return 0;
        }
        sleepingUsers.Remove(userToRemove);
        Console.WriteLine("User Woke: " + user.userID);
        return 1;
    }
 
    private async Task optimizeUserSleep(){
        optimizing = true;
        var timer = new PeriodicTimer(TimeSpan.FromSeconds(60));
        do{
            Parallel.ForEach(sleepingUsers, (user) => {
                _fitbitController.getHeartbeatData(user);
                _nestController.getTemperature(user);
            });
        } while (await timer.WaitForNextTickAsync() & (sleepingUsers.Count() > 0));
        
        optimizing = false;
        return;
    }
}