using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Courts.Commands.DeleteCourt;
public class DeleteCourtHandler : IRequestHandler<DeleteCourtCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _dbContext;

    public DeleteCourtHandler(IBeatSportsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<BeatSportsResponse> Handle(DeleteCourtCommand request, CancellationToken cancellationToken)
    {
        // Check court
        var court = _dbContext.Courts.Where(x => x.Id == request.CourtId).SingleOrDefault();
        if (court == null || court.IsDelete)
        {
            throw new BadRequestException($"Court with Court ID:{request.CourtId} does not exist or have been delele");
        }
        court.IsDelete = true;
        _dbContext.Courts.Update(court);
        _dbContext.SaveChanges();

        return Task.FromResult(new BeatSportsResponse
        {
            Message = "Update successfully!"
        });
    }
}
