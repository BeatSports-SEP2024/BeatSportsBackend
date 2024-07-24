using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Enums;
using BeatSportsAPI.Domain.Entities.Room;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BeatSportsAPI.Application.Features.Rooms.RoomRequests.Commands.CreateRoomRequests;
public class CreateRoomRequestHandler : IRequestHandler<CreateRoomRequestCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;

    public CreateRoomRequestHandler(IBeatSportsDbContext beatSportsDbContext)
    {
        _beatSportsDbContext = beatSportsDbContext;
    }

    public Task<BeatSportsResponse> Handle(CreateRoomRequestCommand request, CancellationToken cancellationToken)
    {
        var roomMatch = _beatSportsDbContext.RoomMatches
            .Where(rm => rm.Id == request.RoomMatchId && !rm.IsDelete)
            .FirstOrDefault();
        var customer = _beatSportsDbContext.Customers
            .Where(c => c.Id == request.CustomerId && !c.IsDelete)
            .FirstOrDefault();

        var roomMatchJoinedList = _beatSportsDbContext.RoomRequests
                                .Where(x => x.CustomerId == customer.Id)
                                .ToList();

        if(roomMatchJoinedList.Count > 0)
        {
            var flag = 0;

            foreach (var room in roomMatchJoinedList)
            {

                var roomMatchCheck = _beatSportsDbContext.RoomMatches
                                .Where(x => x.Id == room.RoomMatchId)
                                .FirstOrDefault();

                if(roomMatch.StartTimeRoom.Date == roomMatchCheck.StartTimeRoom.Date)
                {
                    if (roomMatch.StartTimeRoom <= roomMatchCheck.StartTimeRoom && roomMatch.EndTimeRoom >= roomMatchCheck.EndTimeRoom)
                    {
                        flag++;
                        break;
                    }
                    else if (((roomMatch.StartTimeRoom <= roomMatchCheck.StartTimeRoom) && (roomMatchCheck.StartTimeRoom < roomMatchCheck.EndTimeRoom)))
                    {
                        flag++;
                        break;
                    }
                    else if (((roomMatch.StartTimeRoom < roomMatchCheck.EndTimeRoom) && (roomMatchCheck.EndTimeRoom <= roomMatchCheck.EndTimeRoom)))
                    {
                        flag++;
                        break;
                    }
                }
            }

            if (flag > 0)
            {
                return Task.FromResult(new BeatSportsResponse
                {
                    Message = "400"
                });
            }

        }

        if(roomMatch == null) 
        {
            throw new NotFoundException($"{request.RoomMatchId} is not existed");
        }

        if(customer == null)
        {
            throw new NotFoundException($"{request.CustomerId} is not existed");
        }

        var roomRequest = new RoomRequest
        {
            CustomerId = customer.Id,
            RoomMatchId = roomMatch.Id,
            JoinStatus = RoomRequestEnums.Pending,
            DateRequest = DateTime.UtcNow,
        };
        _beatSportsDbContext.RoomRequests.Add(roomRequest);
        _beatSportsDbContext.SaveChangesAsync(cancellationToken);
        return Task.FromResult(new BeatSportsResponse
        {
            Message = "Send Request To Joining Successful, Waiting for Room Master approve"
        });
    }
}