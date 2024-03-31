using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers;

[ApiController]
[Route("api/c/[controller]")]
public class PlatformsController : ControllerBase
{
    public PlatformsController()
    {

    }

    [HttpPost]
    public ActionResult TestInboundConnection()
    {
        System.Console.WriteLine("--> Inbound POST # Command Service");
        return Ok("Test Ok from Platforms Controller");
    }
}