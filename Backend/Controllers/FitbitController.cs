using Backend.Models;
using Backend.Models.Fitbit;
using RestSharp;
using RestSharp.Authenticators;
using Backend.Helpers;
namespace Backend.Controllers;

public class FitbitController : IFitbitController{

    private readonly IConfiguration _config;
    private readonly IDatabaseController _databaseController;

    private readonly IFitbitAuthenticator _fitbitAuthenticator;


    public FitbitController(IConfiguration config, IDatabaseController databaseController, IFitbitAuthenticator fitbitAuthenticator){
        _config = config;
        _databaseController = databaseController;
        _fitbitAuthenticator = fitbitAuthenticator;
    }
    
    public async Task<FitbitHeartDataResponse?> getHeartbeatData(User user){
        if(user is null){
            throw(new NullReferenceException("user is null"));
        }
        if(user.fitbitData is null){
            throw(new NullReferenceException("User fitbitData is null"));
        }
        await _fitbitAuthenticator.checkAuth(user);
        var client = new RestClient($"https://api.fitbit.com/1/user/{user.fitbitData.fitbitID}/activities/heart/date/2023-04-12/1d.json");
        var request = new RestRequest();
        request.AddHeader("Authorization", $"Bearer {user.fitbitData.accessToken}");
        request.AddHeader("Cookie", "JSESSIONID=003058BABEEF4515D1D707B7233A58E8.fitbit1; fct=3d9ed1888659495c995ecffc78b54532");
        var response = await client.GetAsync<FitbitHeartDataResponse>(request);
        Console.WriteLine(response.ActivitiesHeart[0].DateTime);
        return response;
    }

    public async Task refreshToken(User user){
        if(user is null){
            throw(new NullReferenceException("user is null"));
        }
        if(user.fitbitData is null){
            throw(new NullReferenceException("User fitbitData is null"));
        }
        var client = new RestClient("https://api.fitbit.com/oauth2/token");
        var request = new RestRequest();
        request.AddHeader("Authorization", $"Basic {_config["Fitbit:ServerClientIDClientSecret"]}");
        request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
        request.AddHeader("Cookie", "JSESSIONID=003058BABEEF4515D1D707B7233A58E8.fitbit1; fct=3d9ed1888659495c995ecffc78b54532");
        request.AddParameter("grant_type", "refresh_token");
        request.AddParameter("refresh_token", $"{user.fitbitData.refreshToken}");
        request.AddParameter("client_id", $"{user.fitbitData.fitbitID}");
        var response = await client.PostAsync(request);
        Console.WriteLine(response.Content);

        return;
    }
}