using BeatSportsAPI.Application.Common.Middlewares;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Courts.TimePeriod.Command.CreateTimePeriod;
using BeatSportsAPI.Application.Features.Courts.TimePeriod.Command.DeleteTimePeriod;
using BeatSportsAPI.Application.Features.Courts.TimePeriod.Command.UpdateTimePeriod;
using BeatSportsAPI.Application.Features.Courts.TimePeriod.Queries;
using BeatSportsAPI.Application.Features.Courts.TimePeriod.Queries.GetTimePeriodById;
using BeatSportsAPI.Domain.Enums;
/*using BeatSportsAPI.Application.Features.Courts.TimePeriod.Queries;
using BeatSportsAPI.Application.Features.Courts.TimePeriod.Queries.GetTimePeriodById;*/
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.TimePeriods;

public class TimePeriodController : ApiControllerBase
{
    private readonly IMediator _mediator;
    public TimePeriodController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Route("get-by-time-period-id")]
    [CustomAuthorize(RoleEnums.Customer, RoleEnums.Owner)]
    public async Task<IActionResult> GetTimePeriodById([FromQuery] GetTimePeriodByIdQuery request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = await _mediator.Send(request);

        return Ok(response);
    }
    [HttpGet]
    [CustomAuthorize(RoleEnums.Customer, RoleEnums.Owner)]
    public async Task<IActionResult> GetTimePeriod([FromQuery] GetTimePeriodCommand request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = await _mediator.Send(request);

        return Ok(response);
    }
    [HttpPost]
    [CustomAuthorize(RoleEnums.Owner)]
    public async Task<IActionResult> CreateTimePeriod([FromBody] CreateTimePeriodCommand request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = await _mediator.Send(request);

        return Ok(response);
    }
    [HttpPut]
    [CustomAuthorize(RoleEnums.Owner)]
    public async Task<IActionResult> UpdateTimePeriod([FromBody] UpdateTimePeriodCommand request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = await _mediator.Send(request);
        return Ok(response);
    }
    [HttpDelete]
    [CustomAuthorize(RoleEnums.Owner)]
    public async Task<IActionResult> DeleteTimePeriod([FromQuery] DeleteTimePeriodCommand request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = await _mediator.Send(request);
        return Ok(response);
    }
}
