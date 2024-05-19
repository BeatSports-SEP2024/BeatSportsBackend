using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Courts.Commands.UpdateCourt;
public class UpdateCourtHandler : IRequestHandler<UpdateCourtCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _dbContext;

    public UpdateCourtHandler(IBeatSportsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<BeatSportsResponse> Handle(UpdateCourtCommand request, CancellationToken cancellationToken)
    {
        // Check Owner
        var owner = _dbContext.Owners.Where(x => x.Id == request.OwnerId).SingleOrDefault();
        if (owner == null || owner.IsDelete)
        {
            throw new BadRequestException($"Owner with Oner ID:{request.OwnerId} does not exist or have been delele");
        }
        // Check court
        var court = _dbContext.Courts.Where(x => x.Id == request.CourtId).SingleOrDefault();
        if (court == null || court.IsDelete)
        {
            throw new BadRequestException($"Court with Court ID:{request.CourtId} does not exist or have been delele");
        }
        court.Address = request.Address;
        court.CourtName = request.CourtName;
        court.Description = request.Description;
        court.GoogleMapURLs = request.GoogleMapURLs;
        court.TimeStart = request.TimeStart;
        court.TimeEnd = request.TimeEnd;
        court.PlaceId = request.PlaceId;

        _dbContext.Courts.Update(court);
        _dbContext.SaveChanges();

        return Task.FromResult(new BeatSportsResponse
        {
            Message = "Update successfully!"
        });
    }
}
