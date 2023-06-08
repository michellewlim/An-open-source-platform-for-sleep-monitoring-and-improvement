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
    
    ///<summary>
    /// Gets the most up to data heartrate data from the user's fitbit account
    ///</summary>
    public async Task<FitbitHeartDataResponse?> getHeartbeatData(User user){
        if(user is null){
            throw(new NullReferenceException("user is null"));
        }
        if(user.fitbitData is null || user.fitbitData.accessToken is null){
            throw(new NullReferenceException("User fitbitData is null"));
        }
        await _fitbitAuthenticator.checkAuth(user);
        var todaydate = DateTime.Now;
        var today = todaydate.ToString("yyyy-MM-dd");
        var now = todaydate.ToString("HH:mm");
        var fiveMinutesAgo = (todaydate - new TimeSpan(0,20,0)).ToString("HH:mm");
        Console.WriteLine("Getting heartbeat data");
        var client = new RestClient($"https://api.fitbit.com/1/user/{user.fitbitData.fitbitID}/activities/heart/date/{today}/1d/1sec/time/{fiveMinutesAgo}/{now}.json");
        var request = new RestRequest();
        request.AddHeader("Authorization", $"Bearer {user.fitbitData.accessToken}");
        request.AddHeader("Cookie", "JSESSIONID=003058BABEEF4515D1D707B7233A58E8.fitbit1; fct=3d9ed1888659495c995ecffc78b54532");
        var response = await client.GetAsync<FitbitHeartDataResponse>(request);
        Console.WriteLine(response.ActivitiesHeart[0].DateTime);
        //Read the most recent heartbeat
        if(response.activitiesheartintraday.dataset.Count >= 1){
            var recentBeat = response.activitiesheartintraday.dataset[response.activitiesheartintraday.dataset.Count-1]; 
            Console.WriteLine(recentBeat.time);
            Console.WriteLine(recentBeat.value);
        }
        return response;
    }
}