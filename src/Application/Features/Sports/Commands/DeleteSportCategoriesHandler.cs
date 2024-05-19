using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Common.Exceptions;
using MediatR;

namespace BeatSportsAPI.Application.Features.Sports.Commands;
public class DeleteSportCategoriesHandler : IRequestHandler<DeleteSportCategoriesCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;
    public DeleteSportCategoriesHandler(IBeatSportsDbContext beatSportsDbContext)
    {
        _beatSportsDbContext = beatSportsDbContext;
    }
    public async Task<BeatSportsResponse> Handle(DeleteSportCategoriesCommand request, CancellationToken cancellationToken)
    {
        var existedCategory = _beatSportsDbContext.SportsCategories
            .Where(sc => sc.Id == request.SportCategoryId)
            .FirstOrDefault();
        if(existedCategory == null)
        {
            throw new NotFoundException($"{request.SportCategoryId} does not existed");
        }
        existedCategory.IsDelete = true;
        await _beatSportsDbContext.SaveChangesAsync(cancellationToken);
        return new BeatSportsResponse 
        {
            Message = $"{request.SportCategoryId} is deleted successfully"
        };
    }
}
