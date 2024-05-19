using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Common.Exceptions;
using MediatR;
using BeatSportsAPI.Domain.Entities.CourtEntity;

namespace BeatSportsAPI.Application.Features.Courts.CourtSportCategory.Commands;
public class CreateCourtSportCategoryHandler : IRequestHandler<CreateCourtSportCategoryCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;
    public CreateCourtSportCategoryHandler(IBeatSportsDbContext beatSportsDbContext)
    {
        _beatSportsDbContext = beatSportsDbContext;
    }
    public async Task<BeatSportsResponse> Handle(CreateCourtSportCategoryCommand request, CancellationToken cancellationToken)
    {
        var isValidCourt = _beatSportsDbContext.Courts
            .Where(c => c.Id == request.CourtId && !c.IsDelete)
            .FirstOrDefault();
        var isValidCategory = _beatSportsDbContext.SportsCategories
            .Where(sc => sc.Id == request.SportCategoryId && !sc.IsDelete)
            .FirstOrDefault();
        if (isValidCategory == null || isValidCourt == null) 
        {
            throw new NotFoundException("Court or SportCategory is not existed");
        }
        var response = new BeatSportsAPI.Domain.Entities.CourtEntity.CourtSportCategory
        {
            CourtId = request.CourtId,
            SportCategoryId = request.SportCategoryId,
        };
        _beatSportsDbContext.CourtSportCategories.Add(response);
        await _beatSportsDbContext.SaveChangesAsync(cancellationToken);
        return new BeatSportsResponse
        {
            Message = "Create new sport category court succesfully"
        };
    }
}