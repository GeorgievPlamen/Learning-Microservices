using AutoMapper;
using CommandsService.Data;
using CommandsService.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers;

[ApiController]
[Route("api/c/[controller]")]
public class PlatformsController : ControllerBase
{
    private readonly ICommandRepo _commandRepo;
    private readonly IMapper _mapper;
    public PlatformsController(ICommandRepo commandRepo, IMapper mapper)
    {
        _mapper = mapper;
        _commandRepo = commandRepo;
    }

    [HttpGet]
    public ActionResult<IEnumerable<PlatformReadDTO>> GetPlatforms()
    {
        System.Console.WriteLine("Getting platforms from commandservice");
        var platformItems = _commandRepo.GetAllPlatforms();
        return Ok(_mapper.Map<IEnumerable<PlatformReadDTO>>(platformItems));
    }

    [HttpPost]
    public ActionResult TestInboundConnection()
    {
        System.Console.WriteLine("--> Inbound POST # Command Service");
        return Ok("Test Ok from Platforms Controller");
    }
}