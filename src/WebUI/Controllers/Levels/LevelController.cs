using BeatSportsAPI.Application.Common.Middlewares;
using BeatSportsAPI.Application.Features.Rooms.Levels.Commands;
using BeatSportsAPI.Application.Features.Rooms.Levels.Queries;
using BeatSportsAPI.Application.Features.Sports.Commands;
using BeatSportsAPI.Application.Features.Sports.Queries;
using BeatSportsAPI.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.Levels;

public class LevelController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public LevelController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [CustomAuthorize(RoleEnums.Customer)]
    public async Task<IActionResult> GetAllLevel([FromQuery] GetAllLevelCommand request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = await _mediator.Send(request);

        return Ok(response);
    }
    [HttpPost]
    [CustomAuthorize]
    public async Task<IActionResult> CreateNewLevel(CreateLevelCommand request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = await _mediator.Send(request);

        return Ok(response);
    }
    [HttpPut]
    [CustomAuthorize]
    public async Task<IActionResult> UpdateLevel(UpdateLevelCommand request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = await _mediator.Send(request);

        return Ok(response);
    }
    [HttpDelete]
    [CustomAuthorize]
    public async Task<IActionResult> DeleteLevel([FromQuery] DeleteLevelCommand request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = await _mediator.Send(request);

        return Ok(response);
    }
}
