using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Campaigns.Commands.DeleteCampaign;
using MediatR;

namespace BeatSportsAPI.Application.Features.Rooms.RoomMatches.Commands.DeleteRoomMatches;
public class DeleteRoomMatchesHandler : IRequestHandler<DeleteRoomMatchesCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _dbContext;

    public DeleteRoomMatchesHandler(IBeatSportsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<BeatSportsResponse> Handle(DeleteRoomMatchesCommand request, CancellationToken cancellationToken)
    {
        //check RoomMatch
        var room = _dbContext.RoomMatches.Where(x => x.Id == request.RoomMatchId).SingleOrDefault();
        if (room == null || room.IsDelete)
        {
            throw new BadRequestException($"RoomMath with RoomMath ID:{request.RoomMatchId} does not exist or have been delele");
        }
        room.IsDelete = true;
        _dbContext.RoomMatches.Update(room);
        _dbContext.SaveChanges();

        return Task.FromResult(new BeatSportsResponse
        {
            Message = "Delete RoomMath successfully!"
        });
    }
}
