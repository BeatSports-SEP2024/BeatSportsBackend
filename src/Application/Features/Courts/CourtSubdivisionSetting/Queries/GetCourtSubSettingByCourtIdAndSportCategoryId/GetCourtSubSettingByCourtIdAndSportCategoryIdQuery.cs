using MediatR;

namespace BeatSportsAPI.Application.Features.Courts.CourtSubdivisionSetting.Queries.GetCourtSubSettingByCourtIdAndSportCategoryId;
public class GetCourtSubSettingByCourtIdAndSportCategoryIdQuery : IRequest<List<CourtSubSettingByCourtIdAndSportCategoryIdResponse>>
{
    public Guid CourtId { get; set; }
    public Guid SportCategoryId { get; set; }
}

public class CourtSubSettingByCourtIdAndSportCategoryIdResponse
{
    public Guid CourtSubSettingId { get; set; }
    public string? CourtSubSettingName { get; set; }
    public int QuantityOfCourtSubdivisionOfCourtSubSetting { get; set; }
}
