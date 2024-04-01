using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.DTOs;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlatformsController : ControllerBase
{
    private readonly IPlatformRepo _platformRepo;
    private readonly IMapper _mapper;
    private readonly ICommandDataClient _commandDataClient;
    private readonly IMessageBusClient _messageBusClient;
    public PlatformsController(IMessageBusClient messageBusClient, ICommandDataClient commandDataClient, IPlatformRepo platformRepo, IMapper mapper)
    {
        _messageBusClient = messageBusClient;
        _commandDataClient = commandDataClient;
        _mapper = mapper;
        _platformRepo = platformRepo;
    }

    [HttpGet]
    public ActionResult<IEnumerable<PlatformReadDTO>> GetPlatforms()
    {
        return Ok(_mapper.Map<List<PlatformReadDTO>>(_platformRepo.GetAllPlatforms()));
    }

    [HttpGet("{id}", Name = "GetPlatformById")]
    public ActionResult<PlatformReadDTO> GetPlatformById(int id)
    {
        var res = _mapper.Map<PlatformReadDTO>(_platformRepo.GetPlatformById(id));

        return res != null ? Ok(res) : NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<PlatformReadDTO>> CreatePlatform(PlatformCreateDTO platformCreateDTO)
    {
        var platform = _mapper.Map<Platform>(platformCreateDTO);
        _platformRepo.CreatePlatform(platform);
        _platformRepo.SaveChanges();
        var result = _mapper.Map<PlatformReadDTO>(platform);

        try
        {
            await _commandDataClient.SendPlatformToCommand(result);
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine($"Could not send synchronously: {ex.Message}");
        }

        try
        {
            var platformPublishedDTO = _mapper.Map<PlatformPublishedDTO>(result);
            platformPublishedDTO.Event = "Platform_Published";
            _messageBusClient.PublishNewPlatform(platformPublishedDTO);
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("Couldnt send async msg " + ex);
        }

        return CreatedAtRoute("GetPlatformById", new { Id = platform.Id }, result);
    }
}
