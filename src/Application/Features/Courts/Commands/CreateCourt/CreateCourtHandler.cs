using AutoMapper;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Entities.CourtEntity;
using MediatR;

namespace BeatSportsAPI.Application.Features.Courts.Commands.CreateCourt;
public class CreateCourtHandler : IRequestHandler<CreateCourtCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _dbContext;

    public CreateCourtHandler(IBeatSportsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<BeatSportsResponse> Handle(CreateCourtCommand request, CancellationToken cancellationToken)
    {
        // Check Owner
        var owner = _dbContext.Owners.Where(x => x.Id == request.OwnerId).SingleOrDefault();
        if(owner == null || owner.IsDelete)
        {
            throw new BadRequestException($"Owner with Owner ID:{request.OwnerId} does not exist or have been delele");
        }
        var court = new Court()
        {
            Address = request.Address,
            CourtName = request.CourtName,
            Description = request.Description,
            GoogleMapURLs = request.GoogleMapURLs,
            OwnerId = request.OwnerId,
            TimeStart = request.TimeStart,
            TimeEnd = request.TimeEnd,
            PlaceId = request.PlaceId,
        };
        _dbContext.Courts.Add(court);
        _dbContext.SaveChanges();
        return Task.FromResult(new BeatSportsResponse
        {
            Message = "Create successfully!"
        });
    }
}
