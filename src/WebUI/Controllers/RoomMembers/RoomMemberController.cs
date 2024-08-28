using BeatSportsAPI.Application.Common.Middlewares;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Common.Response.RoomMemberResponse;
using BeatSportsAPI.Application.Features.Rooms.RoomMatches.Queries.GetAllMembersById;
using BeatSportsAPI.Application.Features.Rooms.RoomMatches.Queries.GetAllRoomMatches;
using BeatSportsAPI.Application.Features.Rooms.RoomMatchesUpdateResult;
using BeatSportsAPI.Application.Features.Rooms.RoomMembers.Commands.CreateRoomMembers;
using BeatSportsAPI.Application.Features.Rooms.RoomMembers.Commands.DeleteRoomMembers;
using BeatSportsAPI.Application.Features.Rooms.RoomMembers.Commands.SwapTeamInRoomMembers;
using BeatSportsAPI.Application.Features.Rooms.RoomMembers.Commands.UpdateRoomMembers;
using BeatSportsAPI.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.RoomMembers;

public class RoomMemberController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public RoomMemberController(IMediator mediator)
    {
        _mediator = mediator;
    }
    [HttpGet]
    [Route("customers")]
    [CustomAuthorize(RoleEnums.Customer)]
    public async Task<PaginatedList<RoomMemberResponse>> GetAll([FromQuery] GetAllMemberByIdCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpPost]
    [CustomAuthorize(RoleEnums.Customer)]
    public async Task<BeatSportsResponse> CreateRoomMember(CreateRoomMemberCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpPut]
    [CustomAuthorize(RoleEnums.Customer)]
    public async Task<BeatSportsResponse> UpdateRoomMember(UpdateRoomMemberCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpDelete]
    [CustomAuthorize(RoleEnums.Customer)]
    public async Task<BeatSportsResponse> DeleteAllRoomMembers([FromQuery] DeleteRoomMemberCommand request)
    {
        return await _mediator.Send(request);
    }

    [HttpPost]
    [Route("swap-team")]
    public async Task<BeatSportsResponse> CreateSwapTeam(SwapTeamInRoomMembersCommand request)
    {
        return await _mediator.Send(request);
    }

    [HttpPost]
    [Route("vote-team-win")]
    public async Task<BeatSportsResponse> VoteTeam(RoomMatchesUpdateResultCommand request)
    {
        return await _mediator.Send(request);
    }
}
