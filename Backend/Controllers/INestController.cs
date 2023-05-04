using Backend.Models;
namespace Backend.Controllers;

public interface INestController{

    public Task getTemperature(User user);

    ///<Summary>
    /// Method <c>setTemperature</c>
    /// <para> Sets the temperature of the user's nest in farenheit </para>
    ///</Summary>
    /// <param name="User"> The user whose temperature you want to adjust </param>
    /// <param name="temperature"> The temperature in farenheit </param>
    public Task setTemperature(User user, int temperature);

}