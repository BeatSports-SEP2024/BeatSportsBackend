using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Courts.CourtSportCategory.Commands;
public class UpdateCourtSportCategoryHandler : IRequestHandler<UpdateCourtSportCategoryCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;

    public UpdateCourtSportCategoryHandler(IBeatSportsDbContext beatSportsDbContext)
    {
        _beatSportsDbContext = beatSportsDbContext;
    }

    public async Task<BeatSportsResponse> Handle(UpdateCourtSportCategoryCommand request, CancellationToken cancellationToken)
    {
        var updateCourtSport = _beatSportsDbContext.CourtSportCategories
            .Where(t => t.CourtSubdivisionId == request.CourtSubdivisionId && t.SportCategoryId == request.SportCategoryId)
            .FirstOrDefault();
        if (updateCourtSport == null)
        {
            throw new NotFoundException("CourtSportCategory does not existed");
        }
        foreach (PropertyInfo property in request.GetType().GetProperties())
        {
            var value = property.GetValue(request, null);
            if (value != null)
            {
                PropertyInfo propertyInfo = updateCourtSport.GetType().GetProperty(property.Name);
                if (propertyInfo != null && propertyInfo.CanWrite)
                {
                    propertyInfo.SetValue(updateCourtSport, value, null);
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