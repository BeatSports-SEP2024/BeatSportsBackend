using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Common.Ultilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Courts.CourtSubdivisionSetting.Queries;
public class GetSettingBySportCategoryHandler : IRequestHandler<GetSettingBySportCategoryCommand, List<CourtSubSettingResponse>>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;

    public GetSettingBySportCategoryHandler(IBeatSportsDbContext beatSportsDbContext)
    {
        _beatSportsDbContext = beatSportsDbContext;
    }

    public async Task<List<CourtSubSettingResponse>> Handle(GetSettingBySportCategoryCommand request, CancellationToken cancellationToken)
    {
        var query = _beatSportsDbContext.CourtSubdivisionSettings
            .Where(css => !css.IsDelete);

        // Lọc dựa trên danh mục thể thao được yêu cầu
        if (request.SportCategoriesFilter != null)
        {
            query = query.Include(c => c.SportCategories)
                         .Where(css => css.SportCategories.Name == request.SportCategoriesFilter.GetDescriptionFromEnum());
        }

        var list = await query.Select(c => new CourtSubSettingResponse
        {
            CourtSubSettingId = c.Id,
            TypeSize = c.CourtType,
            SportCategoryId = c.SportCategories.Id,  
            SportCategoryName = c.SportCategories.Name  
        }).ToListAsync(cancellationToken);

        return list;
    }
}
