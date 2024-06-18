using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response.RoomMemberResponse;
using BeatSportsAPI.Domain.Entities.Room;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Rooms.RoomMatches.Queries.GetAllMembersById;
public class GetAllMemberByIdHandler : IRequestHandler<GetAllMemberByIdCommand, PaginatedList<RoomMemberResponse>>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;

    public GetAllMemberByIdHandler(IBeatSportsDbContext beatSportsDbContext)
    {
        _beatSportsDbContext = beatSportsDbContext;
    }

    public Task<PaginatedList<RoomMemberResponse>> Handle(GetAllMemberByIdCommand request, CancellationToken cancellationToken)
    {
        IQueryable<RoomMember> roomMembers = _beatSportsDbContext.RoomMembers
            .Where(rm => rm.RoomMatchId == request.RoomMatchId);

        var roomMemberResponse = roomMembers.Select(c => new RoomMemberResponse
        {
            RoomMatchId = c.RoomMatchId,
            RoleInRoom = c.RoleInRoom,
            CustomerId = c.CustomerId,
        }).PaginatedListAsync(request.PageIndex, request.PageSize);

        return roomMemberResponse;
    }
}