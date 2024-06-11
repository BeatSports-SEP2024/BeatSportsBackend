using BeatSportsAPI.Application.Features.Courts.CourtSportCategory.Commands;
using BeatSportsAPI.Application.Features.Courts.CourtSportCategory.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.Sports;

public class CourtSportCategoryController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public CourtSportCategoryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetCourtSportCategory([FromQuery] GetCourtSportCategoryCommand request) 
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = await _mediator.Send(request);

        return Ok(response);
    }
    [HttpPost]
    public async Task<IActionResult> CreateCourtSportCategory(CreateCourtSportCategoryCommand request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = await _mediator.Send(request);

        return Ok(response);
    }
    [HttpPut]
    public async Task<IActionResult> UpdateCourtSportCategory(UpdateCourtSportCategoryCommand request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = await _mediator.Send(request);

        return Ok(response);
    }
    [HttpDelete]
    public async Task<IActionResult> DeleteCourtSportCategory([FromQuery] DeleteCourtSportCategoryCommand request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = await _mediator.Send(request);

        return Ok(response);
    }
}
