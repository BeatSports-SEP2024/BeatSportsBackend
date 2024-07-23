using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Common.Ultilities;
using BeatSportsAPI.Domain.Entities.Room;
using BeatSportsAPI.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Rooms.RoomMatches.Commands.CreateRoomMatches;
public class CreateRoomMatchesHandler : IRequestHandler<CreateRoomMatchesCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _dbContext;

    public CreateRoomMatchesHandler(IBeatSportsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<BeatSportsResponse> Handle(CreateRoomMatchesCommand request, CancellationToken cancellationToken)
    {
        //check Level
        var level = await _dbContext.Levels.Where(x => x.Id == request.LevelId).SingleOrDefaultAsync();
        if (level == null || level.IsDelete)
        {
            throw new BadRequestException($"Level with Level ID:{request.LevelId} does not exist or have been deleted");
        }

        //check booking
        var booking = await _dbContext.Bookings
            .Include(b => b.CourtSubdivision) 
            .ThenInclude(cs => cs.CourtSubdivisionSettings) 
            .ThenInclude(css => css.SportCategories) 
            .Where(b => b.Id == request.BookingId && !b.IsDelete && b.BookingStatus == BookingEnums.Approved.ToString() && b.IsRoomBooking == false)
            .FirstOrDefaultAsync();

        var isRoomUseBookingId = await _dbContext.RoomMatches
            .Where(rm => rm.BookingId == request.BookingId).FirstOrDefaultAsync();

        if (booking == null || isRoomUseBookingId != null)
        {
            throw new BadRequestException("This booking may be deleted or this booking is used");
        }

        if (booking.CourtSubdivision == null || booking.CourtSubdivision.CourtSubdivisionSettings == null ||
            booking.CourtSubdivision.CourtSubdivisionSettings.SportCategories == null)
        {
            throw new ArgumentException("Invalid booking details, please check CourtSubdivision or CourtSubdivisionSettings or SportCategories");
        }

        string sportCategoryName = booking.CourtSubdivision.CourtSubdivisionSettings.SportCategories.Name.ToString();
        SportCategoriesEnums sportCategoryEnum;

        if (!SportCategoryMapper.SportCategoryMapping.TryGetValue(sportCategoryName, out sportCategoryEnum))
        {
            throw new ArgumentException($"Invalid sport category: {sportCategoryName}");
        }

        var room = new RoomMatch()
        {
            SportCategory = sportCategoryEnum,
            RoomName = request.RoomName,
            BookingId = request.BookingId,
            LevelId = request.LevelId,
            StartTimeRoom = request.StartTimeRoom,
            EndTimeRoom = booking.StartTimePlaying,
            MaximumMember = request.MaximumMember,
            RuleRoom = request.RuleRoom,
            Note = request.Note
        };

        _dbContext.RoomMatches.Add(room);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var roomMember = new RoomMember()
        {
            CustomerId = booking.CustomerId,
            RoomMatchId = room.Id,
            RoleInRoom = RoleInRoomEnums.Master.ToString()
        };

        _dbContext.RoomMembers.Add(roomMember);
        await _dbContext.SaveChangesAsync(cancellationToken);

        booking.IsRoomBooking = true;

        _dbContext.Bookings.Update(booking);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new BeatSportsResponse
        {
            Message = $"Create RoomMatch successfully, roomMatch Id {room.Id}"
        };
    }
}