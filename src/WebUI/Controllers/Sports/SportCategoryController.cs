using BeatSportsAPI.Application.Features.Sports.Commands;
using BeatSportsAPI.Application.Features.Sports.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.Sports;

public class SportCategoryController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public SportCategoryController(IMediator mediator)
    {
        _mediator = mediator;
    }
    [HttpPost]
    public async Task<IActionResult> CreateSportCategory(CreateSportCategoriesCommand request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = await _mediator.Send(request);

        return Ok(response);
    }
    [HttpPut]
    public async Task<IActionResult> UpdateSportCategory(UpdateSportCategoriesCommand request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = await _mediator.Send(request);

        return Ok(response);
    }
    [HttpDelete]
    public async Task<IActionResult> DeleteSportCategory([FromQuery] DeleteSportCategoriesCommand request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = await _mediator.Send(request);

        return Ok(response);
    }
    [HttpGet]
    public async Task<IActionResult> GetSportCategory([FromQuery] GetSportCategoriesCommand request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = await _mediator.Send(request);

        return Ok(response);
    }
}