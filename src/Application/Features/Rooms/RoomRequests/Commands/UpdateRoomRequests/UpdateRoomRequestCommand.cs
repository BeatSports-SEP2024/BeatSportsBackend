using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Entities.Room;
using BeatSportsAPI.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Rooms.RoomRequests.Commands.UpdateRoomRequests;
public class UpdateRoomRequestCommand : IRequest<BeatSportsResponse>
{
    public Guid CustomerId { get; set; }
    public Guid RoomMatchId { get; set; }
}

public class UpdateRoomRequestCommandHandler : IRequestHandler<UpdateRoomRequestCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;

    public UpdateRoomRequestCommandHandler(IBeatSportsDbContext beatSportsDbContext)
    {
        _beatSportsDbContext = beatSportsDbContext;
    }
    public async Task<BeatSportsResponse> Handle(UpdateRoomRequestCommand request, CancellationToken cancellationToken)
    {
        var roomMatch = _beatSportsDbContext.RoomMatches
             .Where(rm => rm.Id == request.RoomMatchId && !rm.IsDelete)
             .FirstOrDefault();
        var customer = _beatSportsDbContext.Customers
            .Where(c => c.Id == request.CustomerId && !c.IsDelete)
            .FirstOrDefault();

        if (roomMatch == null)
        {
            throw new NotFoundException($"{request.RoomMatchId} is not existed");
        }

        if (customer == null)
        {
            throw new NotFoundException($"{request.CustomerId} is not existed");
        }
        var checkMasterId = await _beatSportsDbContext.RoomMembers
            .Where(rm => rm.RoomMatchId == request.RoomMatchId && rm.CustomerId == request.CustomerId && rm.RoleInRoom == RoleInRoomEnums.Master)
            .FirstOrDefaultAsync();
        if (checkMasterId == null)
        {
            var dataRoomMember = await _beatSportsDbContext.RoomMembers.Where(rm => rm.RoomMatchId == roomMatch.Id && rm.CustomerId == request.CustomerId).SingleOrDefaultAsync();
            if (dataRoomMember != null)
            {
                _beatSportsDbContext.RoomMembers.Remove(dataRoomMember);
            }
            var dataRoomRequest = await _beatSportsDbContext.RoomRequests.Where(rm => rm.RoomMatchId == roomMatch.Id && rm.CustomerId == request.CustomerId).SingleOrDefaultAsync();
            if (dataRoomRequest != null)
            {
                _beatSportsDbContext.RoomRequests.Remove(dataRoomRequest);
            }
            await _beatSportsDbContext.SaveChangesAsync();
        }
        else
        {
            var ListRoomMember = await _beatSportsDbContext.RoomMembers.Where(rm => rm.RoomMatchId == roomMatch.Id).ToListAsync();
            _beatSportsDbContext.RoomMembers.RemoveRange(ListRoomMember);

            var ListRoomRequest = await _beatSportsDbContext.RoomRequests.Where(rm => rm.RoomMatchId == roomMatch.Id).ToListAsync();
            _beatSportsDbContext.RoomRequests.RemoveRange(ListRoomRequest);


            var bookingExist = await _beatSportsDbContext.Bookings.Where(b => b.Id == roomMatch.BookingId && b.CustomerId == request.CustomerId).SingleOrDefaultAsync();
            if (bookingExist != null)
            {
                bookingExist.IsRoomBooking = false;
                _beatSportsDbContext.Bookings.Update(bookingExist);
            }

            _beatSportsDbContext.RoomMatches.Remove(roomMatch);
            await _beatSportsDbContext.SaveChangesAsync();
        }

        return new BeatSportsResponse
        {
            Message = "Logout room match successfully!"
        };
    }
}
