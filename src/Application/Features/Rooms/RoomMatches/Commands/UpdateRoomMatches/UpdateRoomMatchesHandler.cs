using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Campaigns.Commands.UpdateCampaign;
using BeatSportsAPI.Domain.Entities.CourtEntity;
using MediatR;

namespace BeatSportsAPI.Application.Features.Rooms.RoomMatches.Commands.UpdateRoomMatches;
public class UpdateRoomMatchesHandler : IRequestHandler<UpdateRoomMatchesCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _dbContext;

    public UpdateRoomMatchesHandler(IBeatSportsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<BeatSportsResponse> Handle(UpdateRoomMatchesCommand request, CancellationToken cancellationToken)
    {
        // Check RoomMath
        var room = _dbContext.RoomMatches.Where(x => x.Id == request.RoomMatchId).SingleOrDefault();
        if (room == null || room.IsDelete)
        {
            throw new BadRequestException($"RoomMatch with RoomMatch ID:{request.RoomMatchId} does not exist or have been delele");
        }

        //Check booking is valid or not deleted 
        var booking = _dbContext.Bookings
            .Where(x => x.Id == request.BookingId && !x.IsDelete)
            .FirstOrDefault();

        room.BookingId = request.BookingId;
        room.RoomName = request.RoomName;
        room.CourtSubdivisionId = request.CourtSubdivisionId;
        room.LevelId = request.LevelId;
        room.StartTimeRoom = request.StartTimeRoom;
        room.EndTimeRoom = request.EndTimeRoom;
        room.MaximumMember = request.MaximumMember;
        room.RuleRoom = request.RuleRoom;
        room.Note = request.Note;   

        _dbContext.RoomMatches.Update(room);
        _dbContext.SaveChanges();

        return Task.FromResult(new BeatSportsResponse
        {
            Message = "Update RoomMatch successfully!"
        });
    }
}
