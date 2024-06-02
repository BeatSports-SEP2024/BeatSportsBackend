using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Campaigns.Commands.CreateCampaign;
using BeatSportsAPI.Domain.Entities;
using BeatSportsAPI.Domain.Entities.CourtEntity;
using BeatSportsAPI.Domain.Entities.Room;
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
        var court = _dbContext.Courts.Where(x => x.Id == request.CourtSubdivisionId).SingleOrDefault();
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

        var room = new RoomMatch()
        {
            CourtSubdivisionId = request.CourtSubdivisionId,
            LevelId = request.LevelId,
            StartTimeRoom = request.StartTimeRoom,
            EndTimeRoom = request.EndTimeRoom,
            MaximumMember = request.MaximumMember,
            RuleRoom = request.RuleRoom,
            Note = request.Note 
        };
        _dbContext.RoomMatches.Add(room);
        _dbContext.SaveChanges();
        return Task.FromResult(new BeatSportsResponse
        {
            Message = "Create RoomMatch successfully!"
        });
    }
}
