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

    /// <summary>
    /// Checks if the user's token is still valid, if not, gets a new one.
    /// </summary>
    public async Task checkAuth(User user){
        var now = DateTime.Now;
        if(user.fitbitData.expires == null || now > user.fitbitData.expires){
            await GetToken(user);
        }

    }

    /// <summary>
    /// Gets a new fitbit access token for the user
    /// </summary>
    async Task GetToken(User user) {
        if(user is null){
            throw(new NullReferenceException("user is null"));
        }
        if(user.fitbitData is null){
            throw(new NullReferenceException("User fitbitData is null"));
        }
        Console.WriteLine("Getting a fitbit token");

        string auth = $"{_config["Fitbit:ServerClientIDClientSecret"]}";

        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.fitbit.com/oauth2/token");
        request.Headers.Add("Authorization", $"Basic {auth}");
        var collection = new List<KeyValuePair<string, string>>();
        collection.Add(new("grant_type", "refresh_token"));
        collection.Add(new("refresh_token", $"{user.fitbitData.refreshToken}"));
        collection.Add(new("client_id", $"{user.fitbitData.fitbitID}"));
        //Console.WriteLine(user.fitbitData.refreshToken);
        //Console.WriteLine(user.fitbitData.fitbitID);
        var content = new FormUrlEncodedContent(collection);
        request.Content = content;
        //Console.WriteLine("Sending Fitbit token request");
        var response = await client.SendAsync(request);
        Console.WriteLine(await response.Content.ReadAsStringAsync());
        response.EnsureSuccessStatusCode();
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
        Console.WriteLine("Got Fitbit token");
    }
}