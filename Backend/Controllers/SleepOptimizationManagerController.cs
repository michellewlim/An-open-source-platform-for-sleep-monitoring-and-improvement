using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Helpers;

namespace Backend.Controllers;

[ApiController]
[Route("Manager")]
public class SleepOptimizationManagerController : ControllerBase{
    
    private readonly ILogger<SleepOptimizationManagerController> _logger;
    private readonly IDatabaseController _databaseController;
    private readonly IOptimizationScheduler _optimizationScheduler;

    public SleepOptimizationManagerController(ILogger<SleepOptimizationManagerController> logger, IDatabaseController databaseController, IOptimizationScheduler optimizationScheduler){
        _logger = logger;
        _databaseController = databaseController;
        _optimizationScheduler = optimizationScheduler;
    }

    [HttpPut()]
    [Route("Sleep")]
    public async Task<IActionResult> Sleep(UserSleepStatePacket packet){
        var user = await _databaseController.getUser(packet.userID);
        var response = await _optimizationScheduler.sleepUser(user);
        if(response == 1)
            return Ok();

        return BadRequest("User Already Sleeping");
    }

    [HttpPut()]
    [Route("Wake")]
    public async Task<IActionResult> Wake(UserSleepStatePacket packet){
        var user = await _databaseController.getUser(packet.userID);
        if(_optimizationScheduler.wakeUser(user) == 1)
            return Ok();
        
        return BadRequest("User Not Asleep");
    }    
}