using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Rooms.RoomRequests.Commands.UpdateRoomRequests;
using BeatSportsAPI.Domain.Entities.Room;
using BeatSportsAPI.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Rooms.RoomRequests.Commands.ApporveRoomRequests;
public class ApporveRoomRequestHandler : IRequestHandler<ApporveRoomRequestCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;

    public ApporveRoomRequestHandler(IBeatSportsDbContext beatSportsDbContext)
    {
        _beatSportsDbContext = beatSportsDbContext;
    }

    public async Task<BeatSportsResponse> Handle(ApporveRoomRequestCommand request, CancellationToken cancellationToken)
    {
        var roomRequest = _beatSportsDbContext.RoomRequests
            .Where(rq => rq.Id == request.RoomRequestId && !rq.IsDelete)
            .FirstOrDefault();

        if (roomRequest == null)
        {
            throw new NotFoundException($"{request.RoomRequestId} is not existed");
        }

        //Chu room approve request do
        roomRequest.JoinStatus = RoomRequestEnums.Accepted;
        roomRequest.DateApprove = DateTime.UtcNow;

        _beatSportsDbContext.RoomRequests.Update(roomRequest);
        //Khi Accepted thi RoomMatch do co them member
        var roomMember = new RoomMember
        {
            CustomerId = roomRequest.CustomerId,
            RoomMatchId = roomRequest.RoomMatchId,
            RoleInRoom = RoleInRoomEnums.Member,
        };
        _beatSportsDbContext.RoomMembers.Add(roomMember);
        await _beatSportsDbContext.SaveChangesAsync(cancellationToken);

        return new BeatSportsResponse
        {
            Message = "Room request approved successfully."
        };
    }
}