using BeatSportsAPI.Application.Common.Middlewares;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Courts.TimeCheckings.Commands;
using BeatSportsAPI.Application.Features.Courts.TimeCheckings.Queries;
using BeatSportsAPI.Application.Features.Courts.TimePeriod.Command.CreateTimePeriod;
using BeatSportsAPI.Application.Features.Feedbacks.Commands.CreateFeedback;
using BeatSportsAPI.Domain.Entities.CourtEntity;
using BeatSportsAPI.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.TimeCheckings;
public class TimeCheckingController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public TimeCheckingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [CustomAuthorize(RoleEnums.Owner)]
    public async Task<BeatSportsResponse> CreateTimeCheckingForCourtSub(CreateLockCourtSubdivisionCommand request)
    {
        return await _mediator.Send(request);
    }

    [HttpGet]
    [CustomAuthorize(RoleEnums.Customer, RoleEnums.Owner)]
    public async Task<List<TimeChecking>> GetAll([FromQuery] GetAllTimeLockedByCourtSubIdCommand request)
    {
        return await _mediator.Send(request);
    }
}
