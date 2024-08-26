using BeatSportsAPI.Application.Common.Middlewares;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Feedbacks.Commands.CreateFeedback;
using BeatSportsAPI.Application.Features.Feedbacks.Commands.DeleteFeedback;
using BeatSportsAPI.Application.Features.Feedbacks.Commands.UpdateFeedback;
using BeatSportsAPI.Application.Features.Feedbacks.Queries.GetAllFeedbacks;
using BeatSportsAPI.Application.Features.Feedbacks.Queries.GetFeedbackById;
using BeatSportsAPI.Application.Features.Rooms.RoomMatches.Commands.CreateRoomMatches;
using BeatSportsAPI.Application.Features.Rooms.RoomMatches.Commands.DeleteRoomMatches;
using BeatSportsAPI.Application.Features.Rooms.RoomMatches.Commands.UpdateRoomMatches;
using BeatSportsAPI.Application.Features.Rooms.RoomMatches.Queries.GetAllRoomMatches;
using BeatSportsAPI.Application.Features.Rooms.RoomMatches.Queries.GetRequestJoinRoom;
using BeatSportsAPI.Application.Features.Rooms.RoomMatches.Queries.GetRoomMatchById;
using BeatSportsAPI.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.RoomMatches;

public class RoomMatchController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public RoomMatchController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [CustomAuthorize(RoleEnums.Customer)]
    public async Task<RoomMatchResponse> Create(CreateRoomMatchesCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpDelete]
    [CustomAuthorize(RoleEnums.Customer)]
    public async Task<BeatSportsResponse> Delete(DeleteRoomMatchesCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpPut]
    [CustomAuthorize(RoleEnums.Customer)]
    public async Task<BeatSportsResponse> Update(UpdateRoomMatchesCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpGet]
    [Route("all")]
    [CustomAuthorize(RoleEnums.Customer)]
    public async Task<RoomRequestsResponseForGetAll> GetAll([FromQuery] GetAllRoomMatchesCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpGet]
    [Route("get-by-roomMatch-id")]
    [CustomAuthorize(RoleEnums.Customer)]
    public async Task<RoomMatchesDetailResponse> GetByRoomMatchId([FromQuery] GetRoomMatchByIdCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpGet]
    [Route("request-in-room-match")]
    [CustomAuthorize(RoleEnums.Customer)]
    public async Task<GetRoomRequestInRoom> GetRoomRequestInRoom([FromQuery] GetRequestInRoomMatchCommand request)
    {
        return await _mediator.Send(request);
    }
}
