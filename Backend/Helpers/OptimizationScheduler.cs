using Backend.Models;

namespace Backend.Helpers;

public class OptimizationScheduler : IOptimizationScheduler{
    
    private List<User> sleepingUsers {get;set;}

    public OptimizationScheduler(){
        sleepingUsers = new List<User>();
        optimizeUserSleep();
    }

    public int sleepUser(User user){
        var userToAdd = sleepingUsers.Find((sleepingUser) => sleepingUser.userID == user.userID);
        if(userToAdd is not null){
            return 0;
        }
        sleepingUsers.Add(user);
        return 1;
    }

    public int wakeUser(User user){
        var userToRemove = sleepingUsers.Find((sleepingUser) => sleepingUser.userID == user.userID);
        if(userToRemove is null){
            return 0;
        }
        sleepingUsers.Remove(userToRemove);
        return 1;
    }
 
    private async Task optimizeUserSleep(){
        var timer = new PeriodicTimer(TimeSpan.FromSeconds(10));

        while (await timer.WaitForNextTickAsync())
        {
            Parallel.ForEach(sleepingUsers, (user) => {
                Console.WriteLine($"I am {user.userID}!");
            });
        }
    }
}