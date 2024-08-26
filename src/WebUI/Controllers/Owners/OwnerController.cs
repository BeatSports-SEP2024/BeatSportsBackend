using BeatSportsAPI.Application.Common.Middlewares;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Customers.Queries;
using BeatSportsAPI.Application.Features.Feedbacks.Commands.DeleteFeedback;
using BeatSportsAPI.Application.Features.Feedbacks.Commands.UpdateFeedback;
using BeatSportsAPI.Application.Features.Owners.Commands.DeleteOwner;
using BeatSportsAPI.Application.Features.Owners.Commands.UpdateOwner;
using BeatSportsAPI.Application.Features.Owners.Queries.GetAllOwners;
using BeatSportsAPI.Application.Features.Owners.Queries.GetownerByIdWithCourt;
using BeatSportsAPI.Application.Features.Owners.Queries.GetOwnerId;
using BeatSportsAPI.Application.Features.Sports.Queries;
using BeatSportsAPI.Domain.Enums;
using CsvHelper.Configuration.Attributes;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebAPI.Controllers.Owners;

public class OwnerController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public OwnerController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpDelete]
    public async Task<BeatSportsResponse> Delete(DeleteOwnerCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpPut]
    [CustomAuthorize(RoleEnums.Owner)]
    public async Task<BeatSportsResponse> Update(UpdateOwnerCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpGet]
    [SwaggerOperation("Get list of owners")]
    [CustomAuthorize(RoleEnums.Admin)]
    public async Task<PaginatedList<OwnerResponse>> GetAll([FromQuery] GetAllOwnersCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpGet("id")]
    [SwaggerOperation("Get owner by Id")]
    [CustomAuthorize(RoleEnums.Owner)]
    public async Task<IActionResult> GetOwnerById([FromQuery] GetOwnerByIdCommand request)
    {
        var response = await _mediator.Send(request);

        return Ok(response);
    }
    [HttpGet("owner-id")]
    [SwaggerOperation("Get owner with relative court by Id")]
    [CustomAuthorize(RoleEnums.Admin)]
    public async Task<IActionResult> GetCourtRelativeOwner([FromQuery] GetOwnerByIdWithCourtCommand request)
    {
        var response = await _mediator.Send(request);

        return Ok(response);
    }
}