using Backend.Models;
namespace Backend.Helpers;

public interface IOptimizer{
    public Task optimizeTemp(User user);
}