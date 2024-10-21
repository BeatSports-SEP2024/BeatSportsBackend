using MediatR;

namespace BeatSportsAPI.Application.Features.Courts.CourtSportCategory.Queries.GetCourtSportCategoryWIthCourtSubByCourtId;
public class GetCourtSportCategoryWIthCourtSubByCourtIdQuery : IRequest<List<CourtSportCategoryWIthCourtSubResponse>>
{
    public Guid CourtId { get; set; }
}

public class CourtSportCategoryWIthCourtSubResponse
{
    public Guid CourtSportCategoryId { get; set; }
    public string? CourtSportCategoryName { get; set; }
}
