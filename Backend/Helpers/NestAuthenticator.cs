using Backend.Models;
using Backend.Models.Nest;
using Backend.Controllers;
using System.Text.Json;
namespace Backend.Helpers;

public class NestAuthenticator : INestAuthenticator{
    private readonly IConfiguration _config;

    readonly IDatabaseController _databaseController;

    public NestAuthenticator(IConfiguration configuration, IDatabaseController databaseController){
        _databaseController = databaseController;
        _config = configuration;
    }

    public async Task checkAuth(User user){
        var now = DateTime.Now;
        if(user.nestData == null || user.nestData.expires < now){
            await getToken(user);
        }
        return;
    }

    private async Task getToken(User user){
        if(user is null){
            throw(new NullReferenceException("user is null"));
        }
        if(user.nestData is null){
            throw(new NullReferenceException("User NestToken is null"));
        }
        Console.WriteLine("Getting a nest token");
        var clientSecret = $"{_config["Nest:Client Secret"]}";
        var clientID = $"{_config["Nest:Nest Project ID"]}";
        var client = new HttpClient();
        var requestText = $"https://www.googleapis.com/oauth2/v4/token?client_id={clientID}&refresh_token={user.nestData.refreshToken}&grant_type=refresh_token";
        Console.WriteLine(requestText);
        var request = new HttpRequestMessage(HttpMethod.Post, requestText);
        var response = await client.SendAsync(request);
        Console.WriteLine(response.StatusCode);
        response.EnsureSuccessStatusCode();
        Console.WriteLine(await response.Content.ReadAsStringAsync());
        var authresp = JsonSerializer.Deserialize<NestRefreshAuthResponse>(await response.Content.ReadAsStringAsync());
        if(authresp == null){
            throw(new NullReferenceException("authresp is null"));
        }
        authresp.user_id = user.userID;
        await _databaseController.refreshNestUserAuth(authresp);
        user.nestData.accessToken = authresp.access_token;
        var now = DateTime.Now;
        var offset = new TimeSpan(0,0,authresp.expires_in);
        user.nestData.expires = now+offset;
        Console.WriteLine("Got Nest Token");
    }
}