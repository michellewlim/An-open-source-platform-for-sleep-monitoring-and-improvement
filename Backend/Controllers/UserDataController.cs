using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Models.Fitbit;
using Backend.Models.Nest;
namespace Backend.Controllers;

[ApiController]
[Route("UserData")]
public class UserDataController : ControllerBase{
    private readonly ILogger<UserDataController> _logger;
    private readonly IDatabaseController _databaseController;

    public UserDataController(ILogger<UserDataController> logger, IDatabaseController databaseController){
        _logger = logger;
        _databaseController = databaseController;
    }

    /// <summary>
    /// Adds a new user to the system. 
    /// </summary>
    [HttpPost()]
    [Route("AddUser")]
    public async Task<IActionResult> addUser([FromBody] UserDataPacket packet){

        await _databaseController.addUser(packet);

        return CreatedAtAction(nameof(addUser), packet);
    }

    /// <summary>
    /// Link's a user's fitbit account to their user account.
    /// </summary>
    [HttpPut()]
    [Route("LinkFitbit")]
    public async Task<IActionResult> linkFitbit([FromBody] FitbitOnboardPacket packet){
        try{
            await _databaseController.getUser(packet.userID);
        }
        catch(Exception e){
            return BadRequest("User does not exist");
        }
        await _databaseController.linkFitbitToUser(packet);
        return Ok();    
    }

    /// <summary>
    /// Link's a user's nest account to their user account.
    /// </summary>
    [HttpPut()]
    [Route("LinkNest")]
    public async Task<IActionResult> linkNest([FromBody] NestOnboardPacket packet){

        try{
            await _databaseController.getUser(packet.userID);
        }
        catch(Exception e){
            return BadRequest("User does not exist");
        }
        await _databaseController.linkNestToUser(packet);
        return Ok();    
    }

    // [HttpPost()]
    // [Route("RefreshFitbitAuth")]
    // public async Task<IActionResult> refreshFitbitAuth([FromBody] FitbitOnboardPacket packet){
    //     await 
        
    //     return Ok();    
    // }

    /// <summary>
    /// returns a list of all valid user ids
    /// </summary>
    [HttpGet()]
    [Route("GetUserIds")]
    public async Task<List<int>> getUserIds(){
        var users = await _databaseController.getUsers();
        var userIDs = users.Select(i => i.userID).ToList();
        return userIDs;
    }

    /// <summary>
    /// returns true if the user exists or false if the user does not exist
    /// </summary>
    [HttpGet()]
    [Route("UserExists")]
    public async Task<bool> userExists([FromBody ]int userID){
        try{
            await _databaseController.getUser(userID);
        }
        catch(Exception e){
            return false;
        }
        return true;
    }

    /// <summary>
    /// Submit the user's sleep survey to end their nights sleep. 
    /// </summary>
    [HttpPost()]
    [Route("SubmitSurvey")]
    public async Task<IActionResult> submitSurvey([FromBody] UserDailyQuizPacket packet){
        User user;
        try{
            user = await _databaseController.getUser(packet.userID);
        }
        catch(Exception e){
            return BadRequest("User does not exist");
        }
        //The last survey is from last night, so the current session is 1 more. 
        var session = await _databaseController.getSleepSession(user) + 1;
        packet.sleepSession = session;
        await _databaseController.addSurvey(packet);

        return CreatedAtAction(nameof(submitSurvey), packet);
    }
}
