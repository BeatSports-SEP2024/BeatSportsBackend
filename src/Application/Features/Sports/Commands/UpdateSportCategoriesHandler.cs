using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Common.Exceptions;
using MediatR;
using System.Reflection;

namespace BeatSportsAPI.Application.Features.Sports.Commands;
public class UpdateSportCategoriesHandler : IRequestHandler<UpdateSportCategoriesCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;
    public UpdateSportCategoriesHandler(IBeatSportsDbContext beatSportsDbContext)
    {
        _beatSportsDbContext = beatSportsDbContext;
    }
    public async Task<BeatSportsResponse> Handle(UpdateSportCategoriesCommand request, CancellationToken cancellationToken)
    {
        var existedCategory = _beatSportsDbContext.SportsCategories
            .Where(sc => sc.Id == request.SportCategoryId)
            .FirstOrDefault();
        if (existedCategory == null)
        {
            throw new NotFoundException("Time Period does not existed");
        }
        foreach (PropertyInfo property in request.GetType().GetProperties())
        {
            var value = property.GetValue(request, null);
            if (value != null)
            {
                PropertyInfo propertyInfo = existedCategory.GetType().GetProperty(property.Name);
                if (propertyInfo != null && propertyInfo.CanWrite)
                {
                    propertyInfo.SetValue(existedCategory, value, null);
                }
            }
        }
        await _beatSportsDbContext.SaveChangesAsync(cancellationToken);
        return new BeatSportsResponse
        {
            Message = "Update Successfully"
        };
    }
}
