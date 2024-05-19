using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Courts.TimePeriod.Command;
using BeatSportsAPI.Application.Features.Courts.TimePeriod.Queries;
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
    [HttpPost]
    public async Task<IActionResult> CreateTimePeriod([FromBody] CreateTimePeriodCommand request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = await _mediator.Send(request);

        return Ok(response);
    }
    [HttpGet]
    public async Task<IActionResult> GetTimePeriod([FromQuery] GetTimePeriodCommand request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = await _mediator.Send(request);

        return Ok(response);
    }
    [HttpPut]
    public async Task<IActionResult> UpdateTimePeriod(UpdateTimePeriodCommand request)
    {
        if(!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = await _mediator.Send(request);
        return Ok(response);
    }
    [HttpDelete]
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
