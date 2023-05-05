using Backend.Models;

namespace Backend.Helpers;
public interface IOptimizationScheduler
{
    /// <summary>
    /// Puts the user to sleep, starts optimization for that user
    /// </summary>
    public Task<int> sleepUser(User user);

    /// <summary>
    /// Wakes the user, stops optimization for that user
    /// </summary>
    public int wakeUser(User user);
}