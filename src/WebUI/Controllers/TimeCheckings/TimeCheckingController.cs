using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Courts.TimeCheckings.Queries;
using BeatSportsAPI.Application.Features.Feedbacks.Commands.CreateFeedback;
using BeatSportsAPI.Domain.Entities.CourtEntity;
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

    [HttpGet]
    public async Task<List<TimeChecking>> GetAll([FromQuery] GetAllTimeLockedByCourtSubIdCommand request)
    {
        return await _mediator.Send(request);
    }
}
