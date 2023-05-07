using Backend.Models;
using Backend.Controllers;
namespace Backend.Helpers;

public class OptimizationScheduler : IOptimizationScheduler{
    
    private readonly IOptimizer _optimizer;

    private List<User> sleepingUsers {get;set;}

    private Boolean optimizing {get;set;}

    public OptimizationScheduler(IOptimizer optimizer){
        _optimizer = optimizer;
        sleepingUsers = new List<User>();
        //optimizeUserSleep();
    }

    public async Task<int> sleepUser(User user){
        var userToAdd = sleepingUsers.Find((sleepingUser) => sleepingUser.userID == user.userID);
        if(userToAdd is not null){
            return 0;
        }
        sleepingUsers.Add(user);
        user.minutesSleeping = 0;
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
                if(user.minutesSleeping >= 5){
                    _optimizer.optimizeTemp(user);
                }
                user.minutesSleeping += 1;
            });
        } while (await timer.WaitForNextTickAsync() & (sleepingUsers.Count() > 0));
        
        optimizing = false;
        return;
    }
}