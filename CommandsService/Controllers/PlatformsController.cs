using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers;

[Route("api/c/[controller]")]
[ApiController]
public class PlatformsController : ControllerBase
{
    private readonly ICommandRepo _repository;
    private readonly IMapper _mapper;
    public PlatformsController(ICommandRepo repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    [HttpGet]
    public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
    {
        Console.WriteLine("--> Getting Platforms from CommandsService");
        var platformItems = _repository.GetAllPlatforms();
        return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItems));
    }

    [HttpPost]
    public ActionResult TestInboundConnection()
    {
        Console.WriteLine("--> Inbound POST # Command Service");
        return Ok("Inbound test of from Platforms Controller");
    }

    [HttpPost("create")]
    public ActionResult<PlatformReadDto> CreatePlatform(PlatformReadDto platformReadDto)
    {
        Console.WriteLine("--> Creating Platform from CommandsService");
        if (_repository.PlatformExists(platformReadDto.Id))
        {
            return BadRequest("Platform already exists");
        }

        var platform = _mapper.Map<Platform>(platformReadDto);
        _repository.CreatePlatform(platform);
        _repository.SaveChanges();
        return Ok(_mapper.Map<PlatformReadDto>(platform));
    }
}