using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Entities.Room;
using MediatR;

namespace BeatSportsAPI.Application.Features.Rooms.Levels.Commands;
public class CreateLevelHandler : IRequestHandler<CreateLevelCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;

    public CreateLevelHandler(IBeatSportsDbContext beatSportsDbContext)
    {
        _beatSportsDbContext = beatSportsDbContext;
    }

    public async Task<BeatSportsResponse> Handle(CreateLevelCommand request, CancellationToken cancellationToken)
    {
        var levels = new Level 
        {
            LevelName = request.LevelName,
            LevelDescription = request.LevelDescription,
        };
        await _beatSportsDbContext.Levels.AddAsync(levels);
        await _beatSportsDbContext.SaveChangesAsync(cancellationToken);
        return new BeatSportsResponse
        {
            Message = $"Create {request.LevelName} successfully"
        };
    }
}
