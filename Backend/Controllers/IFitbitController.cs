using Backend.Models;
using Backend.Models.Fitbit;

namespace Backend.Controllers;

public interface IFitbitController{
    public Task<FitbitHeartDataResponse?> getHeartbeatData(User user);

    
}
