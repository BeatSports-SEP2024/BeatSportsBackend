using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Rooms.Levels.Commands;
public class DeleteLevelHandler : IRequestHandler<DeleteLevelCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;

    public DeleteLevelHandler(IBeatSportsDbContext beatSportsDbContext)
    {
        _beatSportsDbContext = beatSportsDbContext;
    }

    public Task<BeatSportsResponse> Handle(DeleteLevelCommand request, CancellationToken cancellationToken)
    {
        var level = _beatSportsDbContext.Levels
            .Where(lv => lv.Id == request.LevelId && !lv.IsDelete)
            .SingleOrDefault();
        if(level  == null)
        {
            throw new NotFoundException($"{request.LevelId} does not existed");
        } else
        {
            level.IsDelete = true;
        }
        return Task.FromResult(new BeatSportsResponse
        {
            Message = "Delete Successfully"
        });
    }
}
