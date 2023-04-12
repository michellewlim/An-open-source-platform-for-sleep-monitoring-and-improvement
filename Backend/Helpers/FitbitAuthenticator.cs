using RestSharp;
using System.Text.Json;
using Backend.Controllers;
using Backend.Models.Fitbit;
using Backend.Models;

namespace Backend.Helpers;

public class FitbitAuthenticator : IFitbitAuthenticator {
    private readonly IConfiguration _config;
    private readonly IDatabaseController _databaseController;

    public FitbitAuthenticator(IConfiguration config, IDatabaseController databaseController){
        _config = config;
        _databaseController = databaseController;
    }

    public async Task checkAuth(User user){
        var now = DateTime.Now;
        if(user.fitbitData.expires == null || now > user.fitbitData.expires){
            await GetToken(user);
        }

    }

    // async Task GetToken(User user) {
    //     if(user is null){
    //         throw(new NullReferenceException("user is null"));
    //     }
    //     if(user.fitbitData is null){
    //         throw(new NullReferenceException("User fitbitData is null"));
    //     }
    //     Console.WriteLine("Getting a token");
    //     var options = new RestClientOptions("https://api.fitbit.com"){
    //         ThrowOnAnyError = true,
    //         MaxTimeout = 1000
    //     };
    //     using var client = new RestClient(options);

    //     var request = new RestRequest("/oauth2/token", Method.Post);
    //     string auth = $"{_config["Fitbit:ServerClientIDClientSecret"]}";
    //     request.AddHeader("Authorization", $"Basic {auth}");
    //     request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
    //     request.AddParameter("grant_type", "refresh_token");
    //     request.AddParameter("refresh_token", $"{user.fitbitData.refreshToken}");
    //     request.AddParameter("client_id", $"{user.fitbitData.fitbitID}");
    //     Console.WriteLine("Sending request");
    //     var response = await client.ExecuteAsync<FitbitRefreshAuthResponse>(request);
    //     Console.WriteLine(response.StatusCode);
    //     if(response.Data == null){
    //         throw(new NullReferenceException("response.Data is null"));
    //     }
    //     await _databaseController.refreshFitbitUserAuth(response.Data);
    //     user.fitbitData.accessToken = response.Data.accessToken;
    //     user.fitbitData.refreshToken = response.Data.refreshToken;
    //     Console.WriteLine(response.Data.accessToken);
    //     Console.WriteLine(response.Data.refreshToken);
    //     var now = DateTime.Now;
    //     var offset = new TimeSpan(0,0,Int32.Parse(response.Data.expires_in));
    //     user.fitbitData.expires = now + offset;
    //     Console.WriteLine("Got token");
    // }
async Task GetToken(User user) {
        if(user is null){
            throw(new NullReferenceException("user is null"));
        }
        if(user.fitbitData is null){
            throw(new NullReferenceException("User fitbitData is null"));
        }
        Console.WriteLine("Getting a token");

        string auth = $"{_config["Fitbit:ServerClientIDClientSecret"]}";

        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.fitbit.com/oauth2/token");
        request.Headers.Add("Authorization", $"Basic {auth}");
        var collection = new List<KeyValuePair<string, string>>();
        collection.Add(new("grant_type", "refresh_token"));
        collection.Add(new("refresh_token", $"{user.fitbitData.refreshToken}"));
        collection.Add(new("client_id", $"{user.fitbitData.fitbitID}"));
        var content = new FormUrlEncodedContent(collection);
        request.Content = content;
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        Console.WriteLine(await response.Content.ReadAsStringAsync());
        var authresp = JsonSerializer.Deserialize<FitbitRefreshAuthResponse>(await response.Content.ReadAsStringAsync());
        Console.WriteLine(response.StatusCode);
        
        if(authresp == null){
            throw(new NullReferenceException("authresp is null"));
        }
        await _databaseController.refreshFitbitUserAuth(authresp);
        user.fitbitData.accessToken = authresp.access_token;
        user.fitbitData.refreshToken = authresp.access_token;
        var now = DateTime.Now;
        var offset = new TimeSpan(0,0,(authresp.expires_in));
        user.fitbitData.expires = now + offset;
        Console.WriteLine("Got token");
    }

}