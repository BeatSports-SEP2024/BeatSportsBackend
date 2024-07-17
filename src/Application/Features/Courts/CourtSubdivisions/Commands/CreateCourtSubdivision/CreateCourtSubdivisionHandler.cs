using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Entities.CourtEntity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace BeatSportsAPI.Application.Features.Courts.CourtSubdivisions.Commands.CreateCourtSubdivision;
public class CreateCourtSubdivisionHandler : IRequestHandler<CreateCourtSubdivisionCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _dbContext;

    public CreateCourtSubdivisionHandler(IBeatSportsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<BeatSportsResponse> Handle(CreateCourtSubdivisionCommand request, CancellationToken cancellationToken)
    {
            //Check court is valid
            var court = _dbContext.Courts
                .Where(c => c.Id == request.CourtId && !c.IsDelete)
                .SingleOrDefault();

            if( court == null ) 
            {
                throw new BadRequestException("This sport court does not exist");
            }

            var courtSubdivision = new CourtSubdivision
            {
                CourtId = request.CourtId,
                BasePrice = request.BasePrice,
                //ImageURL = command.ImageURL,
                //Description = command.Description,
                CourtSubdivisionDescription = request.CourtSubDescription,
                IsActive = true,
                CourtSubdivisionName = request.CourtSubdivisionName,
            };
            _dbContext.CourtSubdivisions.Add(courtSubdivision);

        await _dbContext.SaveChangesAsync(cancellationToken);
        return new BeatSportsResponse
        {
            Message = "Create successfully"
        };
    }
}