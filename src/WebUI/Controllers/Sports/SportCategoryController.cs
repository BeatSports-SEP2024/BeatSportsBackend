using BeatSportsAPI.Application.Common.Middlewares;
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
    [HttpGet]
    [CustomAuthorize]
    public async Task<IActionResult> GetSportCategory([FromQuery] GetSportCategoriesCommand request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = await _mediator.Send(request);

        return Ok(response);
    }
    [HttpGet("id")]
    [CustomAuthorize]
    public async Task<IActionResult> GetSportCategoryById([FromQuery] GetSportCategoryByIdCommand request)
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
    [CustomAuthorize]
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
    [CustomAuthorize]
    public async Task<IActionResult> DeleteSportCategory([FromQuery] DeleteSportCategoriesCommand request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = await _mediator.Send(request);

        return Ok(response);
    }  
}