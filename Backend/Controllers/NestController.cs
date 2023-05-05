using Backend.Models;
using Backend.Helpers;
namespace Backend.Controllers;

public class NestController : INestController{

    private readonly IConfiguration _config;

    private readonly INestAuthenticator _nestAuthenticator;

    readonly IDatabaseController _databaseController;

    public NestController(IConfiguration configuration, IDatabaseController databaseController, INestAuthenticator nestAuthenticator){;
        _databaseController = databaseController;
        _config = configuration;
        _nestAuthenticator = nestAuthenticator;
    }

    public async Task<double> getTemperature(User user){
        await _nestAuthenticator.checkAuth(user);
        
        return 0.0;
    }

    public async Task setTemperature(User user, int temperature){
        await _nestAuthenticator.checkAuth(user);

        return;
    }

}