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
    [Route("SubmitData")]
    public async Task<IActionResult> SubmitData([FromBody] UserDataPacket packet){

        await _databaseController.addData(packet);

        return CreatedAtAction(nameof(SubmitData), packet);
    }

    [HttpGet]
    public IActionResult Test(){
        return Ok();
    }

}