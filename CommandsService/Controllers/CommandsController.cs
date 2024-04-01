using System.ComponentModel.Design;
using AutoMapper;
using CommandsService.Data;
using CommandsService.DTOs;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers;

[ApiController]
[Route("api/c/platforms/{platformId}/[controller]")]
public class CommandsController : ControllerBase
{
    private readonly ICommandRepo _commandRepo;
    private readonly IMapper _mapper;
    public CommandsController(ICommandRepo commandRepo, IMapper mapper)
    {
        _mapper = mapper;
        _commandRepo = commandRepo;
    }

    [HttpGet]
    public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform(int platformId)
    {
        System.Console.WriteLine("Hit GetCommandsForPlatform");

        if (_commandRepo.PlatformExists(platformId) == false)
        {
            return NotFound();
        }

        var commands = _commandRepo.GetCommandsForPlatform(platformId);

        return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));
    }

    [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
    public ActionResult<CommandReadDto> GetCommandForPlatform(int platformId, int commandId)
    {
        if (_commandRepo.PlatformExists(platformId) == false)
        {
            return NotFound();
        }

        var command = _commandRepo.GetCommand(platformId, commandId);

        if (command == null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<CommandReadDto>(command));
    }

    [HttpPost]
    public ActionResult<CommandReadDto> CreateCommandForPlatform(int platformId, CommandCreateDTO commandCreateDTO)
    {
        if (_commandRepo.PlatformExists(platformId) == false)
        {
            return NotFound();
        }

        var command = _mapper.Map<Command>(commandCreateDTO);

        _commandRepo.CreateCommand(platformId, command);
        _commandRepo.SaveChanges();

        var commandReadDto = _mapper.Map<CommandReadDto>(command);
        return CreatedAtRoute(nameof(GetCommandForPlatform),
            new { platformId = platformId, commandId = commandReadDto.Id }, commandReadDto);
    }
}