using Backend.Models;

namespace Backend.Helpers;
public interface IOptimizationScheduler
{
    public int sleepUser(User user);

    public int wakeUser(User user);
}