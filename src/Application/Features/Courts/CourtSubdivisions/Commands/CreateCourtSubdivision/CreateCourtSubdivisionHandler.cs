using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Entities.CourtEntity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace BeatSportsAPI.Application.Features.Courts.CourtSubdivisions.Commands.CreateCourtSubdivision;
public class CreateCourtSubdivisionHandler : IRequestHandler<CreateListCourtSubdivisionCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _dbContext;

    public CreateCourtSubdivisionHandler(IBeatSportsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<BeatSportsResponse> Handle(CreateListCourtSubdivisionCommand request, CancellationToken cancellationToken)
    {
        foreach(var command in request.CreateListCourtSubCommands)
        {
            //Check court is valid
            var court = _dbContext.Courts
                .Where(c => c.Id == command.CourtId && !c.IsDelete)
                .SingleOrDefault();

            if( court == null ) 
            {
                throw new BadRequestException("This sport court does not exist");
            }

            var courtSubdivision = new CourtSubdivision
            {
                CourtId = command.CourtId,
                BasePrice = command.BasePrice,
                //ImageURL = command.ImageURL,
                //Description = command.Description,
                IsActive = true,
                CourtSubdivisionName = command.CourtSubdivisionName,
            };
            _dbContext.CourtSubdivisions.Add(courtSubdivision);
        }
        await _dbContext.SaveChangesAsync(cancellationToken);
        return new BeatSportsResponse
        {
            Message = "Create successfully"
        };
    }
}