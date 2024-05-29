using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Common.Response.RoomMemberResponse;
using BeatSportsAPI.Application.Features.Rooms.RoomMembers.Commands.CreateRoomMembers;
using BeatSportsAPI.Application.Features.Rooms.RoomMembers.Commands.DeleteRoomMembers;
using BeatSportsAPI.Application.Features.Rooms.RoomMembers.Commands.UpdateRoomMembers;
using BeatSportsAPI.Application.Features.Rooms.RoomMembers.Queries.GetAllRoomMember;
using BeatSportsAPI.Application.Features.Rooms.RoomMembers.Queries.GetAllRoomMemberDetails;
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
    public async Task<PaginatedList<RoomMemberResponse>> GetAllRoomMember([FromQuery] GetAllRoomMemberCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpGet]
    [Route("details")]
    public async Task<PaginatedList<RoomMemberWithDetailsResponse>> GetAllRoomMemberWithDetails([FromQuery] GetAllRoomMemberWithDetailCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpPost]
    public async Task<BeatSportsResponse> CreateRoomMember(CreateRoomMemberCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpPut]
    public async Task<BeatSportsResponse> UpdateRoomMember(UpdateRoomMemberCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpDelete]
    public async Task<BeatSportsResponse> DeleteAllRoomMembers([FromQuery] DeleteRoomMemberCommand request)
    {
        return await _mediator.Send(request);
    }
}
