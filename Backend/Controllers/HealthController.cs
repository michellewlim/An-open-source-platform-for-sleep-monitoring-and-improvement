using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("Health")]
public class HealthController : ControllerBase{

    [HttpGet]
    public IActionResult Test(){
        return Ok();
    }

    [HttpGet]
    [Route("Sum/{value1}/{value2}")]
    public IActionResult Sum(int value1, int value2){
        return Ok(value1 + value2);
    }
}