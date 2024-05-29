using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Rooms.Levels.Commands;
public class UpdateLevelHandler : IRequestHandler<UpdateLevelCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;

    public UpdateLevelHandler(IBeatSportsDbContext beatSportsDbContext)
    {
        _beatSportsDbContext = beatSportsDbContext;
    }

    public async Task<BeatSportsResponse> Handle(UpdateLevelCommand request, CancellationToken cancellationToken)
    {
        var level = _beatSportsDbContext.Levels
            .Where(lv => lv.Id == request.LevelId && !lv.IsDelete)
            .SingleOrDefault();
        if (level == null)
        {
            throw new NotFoundException($"{request.LevelId} does not existed");
        } else
        {
            level.LevelName = request.LevelName;
            level.LevelDescription = request.LevelDescription;
        }
        _beatSportsDbContext.Levels.Update(level);
        await _beatSportsDbContext.SaveChangesAsync(cancellationToken);
        return new BeatSportsResponse
        {
            Message = $"{request.LevelId} is updated"
        };
    }
}
