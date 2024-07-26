using BeatSportsAPI.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Courts.CourtSubdivisionSetting.Queries.GetCourtSubSettingByCourtIdAndSportCategoryId;
public class GetCourtSubSettingByCourtIdAndSportCategoryIdQueryHandler : IRequestHandler<GetCourtSubSettingByCourtIdAndSportCategoryIdQuery, List<CourtSubSettingByCourtIdAndSportCategoryIdResponse>>
{
    private readonly IBeatSportsDbContext _dbContext;

    public GetCourtSubSettingByCourtIdAndSportCategoryIdQueryHandler(IBeatSportsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<CourtSubSettingByCourtIdAndSportCategoryIdResponse>> Handle(GetCourtSubSettingByCourtIdAndSportCategoryIdQuery request, CancellationToken cancellationToken)
    {
        // Lấy list court sub setting by sport category id
        var joinList = await (from cs in _dbContext.CourtSubdivisions

                              join css in _dbContext.CourtSubdivisionSettings

                              on cs.CourtSubdivisionSettingId equals css.Id

                              where cs.CourtId == request.CourtId &&
                                    css.SportCategoryId == request.SportCategoryId &&
                                    !cs.IsDelete && !css.IsDelete
                              group cs by new { css.Id, css.CourtType } into g
                              select new CourtSubSettingByCourtIdAndSportCategoryIdResponse
                              {
                                  CourtSubSettingId = g.Key.Id,
                                  CourtSubSettingName = g.Key.CourtType,
                                  QuantityOfCourtSubdivisionOfCourtSubSetting = g.Count()
                              }).ToListAsync();
        return joinList;
    }
}
