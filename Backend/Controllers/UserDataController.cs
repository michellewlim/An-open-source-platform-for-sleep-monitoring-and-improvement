using Microsoft.AspNetCore.Mvc;
using Backend.Models;

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

    [HttpPost()]
    [Route("AddUser")]
    public async Task<IActionResult> addUser([FromBody] UserDataPacket packet){

        await _databaseController.addUser(packet);

        return CreatedAtAction(nameof(addUser), packet);
    }

    [HttpPut()]
    [Route("LinkFitbit")]
    public async Task<IActionResult> linkFitbit([FromBody] FitbitOnboardPacket packet){
        await _databaseController.linkFitbitToUser(packet);
        return Ok();    
    }

    [HttpGet()]
    [Route("GetUserIds")]
    public async Task<List<User>> getUserIds(){
        return await _databaseController.getUsers();
    }

    [HttpPost()]
    [Route("SubmitSurvey")]
    public async Task<IActionResult> submitSurvey([FromBody] UserDailyQuizPacket packet){
        
        await _databaseController.addSurvey(packet);

        return CreatedAtAction(nameof(submitSurvey), packet);
    }
}
