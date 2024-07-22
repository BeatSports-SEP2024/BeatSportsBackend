using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Entities.Room;
using BeatSportsAPI.Domain.Enums;
using MediatR;

namespace BeatSportsAPI.Application.Features.Rooms.RoomMatches.Commands.CreateRoomMatches;
public class CreateRoomMatchesHandler : IRequestHandler<CreateRoomMatchesCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _dbContext;

    public CreateRoomMatchesHandler(IBeatSportsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<BeatSportsResponse> Handle(CreateRoomMatchesCommand request, CancellationToken cancellationToken)
    {
        //check Court
        var court = _dbContext.CourtSubdivisions.Where(x => x.Id == request.CourtSubdivisionId).SingleOrDefault();
        if (court == null || court.IsDelete)
        {
            throw new BadRequestException($"Court with Court ID:{request.CourtSubdivisionId} does not exist or have been delele");
        }

        //check Level
        var level = _dbContext.Levels.Where(x => x.Id == request.LevelId).SingleOrDefault();
        if (level == null || level.IsDelete)
        {
            throw new BadRequestException($"Level with Level ID:{request.LevelId} does not exist or have been delele");
        }

        //check booking
        var booking = _dbContext.Bookings
            .Where(b => b.Id == request.BookingId && !b.IsDelete && b.IsRoomBooking == true && b.BookingStatus == BookingEnums.Approved.ToString()).FirstOrDefault();

        var isRoomUseBookingId = _dbContext.RoomMatches
            .Where(rm => rm.BookingId == request.BookingId).FirstOrDefault();

        if(booking == null || isRoomUseBookingId != null)
        {
            throw new BadRequestException("This booking maybe is deleted or this booking is used");
        }

        var room = new RoomMatch()
        {
            RoomName = request.RoomName,
            BookingId = request.BookingId,
            LevelId = request.LevelId,
            StartTimeRoom = request.StartTimeRoom,
            EndTimeRoom = request.EndTimeRoom,
            MaximumMember = request.MaximumMember,
            RuleRoom = request.RuleRoom,
            Note = request.Note 
        };
        _dbContext.RoomMatches.Add(room);
        _dbContext.SaveChanges();
        var roomMember = new RoomMember()
        {
            CustomerId = booking.CustomerId,
            RoomMatchId = room.Id,
            RoleInRoom = RoleInRoomEnums.Master.ToString()
        };

        _dbContext.RoomMembers.Add(roomMember);
        _dbContext.SaveChanges();
        return Task.FromResult(new BeatSportsResponse
        {
            Message = "Create RoomMatch successfully!"
        });
    }
}
