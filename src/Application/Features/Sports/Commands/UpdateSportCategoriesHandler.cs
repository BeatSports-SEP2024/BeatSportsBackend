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
            throw new NotFoundException("Sport category does not existed");
        }
        foreach (PropertyInfo requestProperty in request.GetType().GetProperties())
        {
            var requestValue = requestProperty.GetValue(request, null);
            if (requestValue != null)
            {
                PropertyInfo courtSportProperty = existedCategory.GetType().GetProperty(requestProperty.Name);
                if (courtSportProperty != null && courtSportProperty.CanWrite)
                {
                    if (courtSportProperty.PropertyType.IsEnum)
                    {
                        // Chuyển đổi giá trị từ request sang kiểu enum tương ứng
                        var enumValue = Enum.Parse(courtSportProperty.PropertyType, requestValue.ToString());
                        courtSportProperty.SetValue(existedCategory, enumValue, null);
                    }
                    else if (courtSportProperty.PropertyType == typeof(string) && requestProperty.PropertyType.IsEnum)
                    {
                        // Chuyển đổi giá trị enum từ request sang string
                        var stringValue = requestValue.ToString();
                        courtSportProperty.SetValue(existedCategory, stringValue, null);
                    }
                    else
                    {
                        courtSportProperty.SetValue(existedCategory, requestValue, null);
                    }
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
