﻿using BeatSportsAPI.Application.Common.Exceptions;
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
            .Where(rq => rq.Id == request.RoomRequestId && rq.CustomerId == request.CustomerId && !rq.IsDelete)
            .FirstOrDefault();

        if (roomRequest == null)
        {
            throw new NotFoundException($"{request.RoomRequestId} is not existed");
        }

        switch (request.RoomRequest.ToString())
        {
            case "Accepted":
                // Chủ phòng chấp nhận yêu cầu
                roomRequest.JoinStatus = RoomRequestEnums.Accepted;
                roomRequest.DateApprove = DateTime.UtcNow;

                _beatSportsDbContext.RoomRequests.Update(roomRequest);
                // Khi được chấp nhận, thì RoomMatch có thêm thành viên
                var roomMember = new RoomMember
                {
                    CustomerId = roomRequest.CustomerId,
                    RoomMatchId = roomRequest.RoomMatchId,
                    RoleInRoom = RoleInRoomEnums.Member,
                };
                _beatSportsDbContext.RoomMembers.Add(roomMember);
                break;

            case "Declined":
                //roomRequest.JoinStatus = RoomRequestEnums.Declined;
                //roomRequest.DateApprove = DateTime.UtcNow;

                _beatSportsDbContext.RoomRequests.Remove(roomRequest);
                break;
        }

        await _beatSportsDbContext.SaveChangesAsync(cancellationToken);

        return new BeatSportsResponse
        {
            Message = roomRequest.JoinStatus == RoomRequestEnums.Accepted ? "Room request approved successfully." : "Room request declined."
        };
    }
}