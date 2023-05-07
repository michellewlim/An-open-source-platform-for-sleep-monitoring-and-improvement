using Backend.Models;
using Backend.Models.Nest;
namespace Backend.Controllers;

public interface INestController{

    ///<summary>
    /// gets the current ambient temperature of the user's nest in celcius
    ///</summary>
    public Task<NestDevicesPacket> getTemperature(User user);

    ///<Summary>
    /// Method <c>setTemperature</c>
    /// <para> Sets the temperature of the user's nest in celcius </para>
    ///</Summary>
    /// <param name="User"> The user whose temperature you want to adjust </param>
    /// <param name="temperature"> The temperature in celcius </param>
    public Task setTemperature(User user, double temperature, string deviceID);

}