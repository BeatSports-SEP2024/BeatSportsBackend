using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Rooms.RoomMatches.Queries.GetRequestJoinRoom;
public class GetRequestInRoomMatchHandler : IRequestHandler<GetRequestInRoomMatchCommand, GetRoomRequestInRoom>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;
    public GetRequestInRoomMatchHandler(IBeatSportsDbContext beatSportsDbContext)
    {
        _beatSportsDbContext = beatSportsDbContext;
    }

    public Task<GetRoomRequestInRoom> Handle(GetRequestInRoomMatchCommand request, CancellationToken cancellationToken)
    {
        var roomMatchQuery = _beatSportsDbContext.RoomMatches
            .Include(rm => rm.RoomRequests)
                .ThenInclude(req => req.Customer)
                    .ThenInclude(cus => cus.Account)
            .Where(rm => rm.Id == request.RoomMatchId)
            .AsQueryable();

        var result = roomMatchQuery.Select(rm => new GetRoomRequestInRoom
        {
            RoomMatchId = rm.Id,
            JoiningRequest = rm.RoomRequests.Select(req => new RoomRequestInRoom
            {
                CustomerAvatar = req.Customer.Account.ProfilePictureURL,
                RoomRequestsId = req.Id,
                CustomerName = req.Customer.Account.FirstName + " " + req.Customer.Account.LastName,
                CustomerId = req.Customer.Id,
            }).ToList()
        }).FirstOrDefaultAsync(cancellationToken);

        return result;
    }
}