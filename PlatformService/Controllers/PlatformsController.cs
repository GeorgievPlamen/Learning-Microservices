using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.DTOs;
using PlatformService.Models;

namespace PlatformService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlatformsController : ControllerBase
{
    private readonly IPlatformRepo _platformRepo;
    private readonly IMapper _mapper;
    public PlatformsController(IPlatformRepo platformRepo, IMapper mapper)
    {
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
    public ActionResult<PlatformReadDTO> CreatePlatform(PlatformCreateDTO platformCreateDTO)
    {
        var platform = _mapper.Map<Platform>(platformCreateDTO);
        _platformRepo.CreatePlatform(platform);
        _platformRepo.SaveChanges();
        var result = _mapper.Map<PlatformReadDTO>(platform);

        return CreatedAtRoute("GetPlatformById", new { Id = platform.Id }, result);
    }
}
