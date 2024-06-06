using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Entities.CourtEntity;
using MediatR;

namespace BeatSportsAPI.Application.Features.Courts.CourtSubdivisions.Commands.CreateCourtSubdivision;
public class CreateCourtSubdivisionHandler : IRequestHandler<CreateCourtSubdivisionCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _dbContext;

    public CreateCourtSubdivisionHandler(IBeatSportsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<BeatSportsResponse> Handle(CreateCourtSubdivisionCommand request, CancellationToken cancellationToken)
    {
        //check court
        var court = _dbContext.Courts.Where(x => x.Id == request.CourtId).SingleOrDefault();
        if (court == null || court.IsDelete)
        {
            throw new BadRequestException($"Court with court ID:{request.CourtId} does not exist or have been delele");
        }
        var courtSubdivision = new CourtSubdivision
        {
            CourtId = request.CourtId,
            BasePrice = request.BasePrice,
            ImageURL = request.ImageURL,
            Description = request.Description,
            IsActive = true,
            CourtSubdivisionName = request.CourtSubdivisionName,
        };
        _dbContext.CourtSubdivisions.Add(courtSubdivision);
        _dbContext.SaveChanges();
        return Task.FromResult(new BeatSportsResponse
        {
            Message = "Create successfully"
        });
    }
}
