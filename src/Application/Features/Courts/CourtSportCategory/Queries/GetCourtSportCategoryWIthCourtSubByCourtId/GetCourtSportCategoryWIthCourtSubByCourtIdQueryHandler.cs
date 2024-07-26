using BeatSportsAPI.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Courts.CourtSportCategory.Queries.GetCourtSportCategoryWIthCourtSubByCourtId;
public class GetCourtSportCategoryWIthCourtSubByCourtIdQueryHandler : IRequestHandler<GetCourtSportCategoryWIthCourtSubByCourtIdQuery, List<CourtSportCategoryWIthCourtSubResponse>>
{
    private readonly IBeatSportsDbContext _dbContext;

    public GetCourtSportCategoryWIthCourtSubByCourtIdQueryHandler(IBeatSportsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<CourtSportCategoryWIthCourtSubResponse>> Handle(GetCourtSportCategoryWIthCourtSubByCourtIdQuery request, CancellationToken cancellationToken)
    {
        // Vì kh thấy bảng Sport category nối với bảng court mà
        // thông qua court setting cũng như từ court setting
        // tới court sub, vì vậy ta sẽ đi ngược lại
        var court = await _dbContext.CourtSubdivisions.Where(x => x.CourtId == request.CourtId && !x.IsDelete)
            .Include(x => x.CourtSubdivisionSettings)
            .ThenInclude(x => x.SportCategories).ToListAsync();
        var response = new List<CourtSportCategoryWIthCourtSubResponse>();
        foreach (var item in court)
        {
            var sportCategoryId = item.CourtSubdivisionSettings.SportCategories.Id;
            // Nếu khôn tồn tại thì add vô cái mới
            if (!response.Select(x => x.CourtSportCategoryId).ToList().Contains(sportCategoryId))
            {
                var responseItem = new CourtSportCategoryWIthCourtSubResponse
                {
                    CourtSportCategoryId = sportCategoryId,
                    CourtSportCategoryName = item.CourtSubdivisionSettings.SportCategories.Name
                };
                response.Add(responseItem);
            }
        }
        return response;
    }
}
