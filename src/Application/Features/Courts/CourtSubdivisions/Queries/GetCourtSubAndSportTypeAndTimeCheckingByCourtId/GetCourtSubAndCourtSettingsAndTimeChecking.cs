using BeatSportsAPI.Application.Features.Courts.CourtSubdivisions.Queries.GetCourtSubdivisionAndTimeByCourtIdAndDate;
using MediatR;

namespace BeatSportsAPI.Application.Features.Courts.CourtSubdivisions.Queries.GetCourtSubAndSportTypeAndTimeCheckingByCourtId;
public class GetCourtSubAndCourtSettingsAndTimeChecking : IRequest<ResponseCourtDataInCourtSubAndCourtSettingsAndTimeChecking>
{
    public Guid CourtId { get; set; }
    public DateTime DateCheck { get; set; }
}

public class ResponseCourtDataInCourtSubAndCourtSettingsAndTimeChecking
{
    public Guid CourtId { get; set; }
    public string TimeStart { get; set; } = null!;
    public string TimeEnd { get; set; } = null!;
    public List<CourtSubSettingResponseV2> CourtSubSettingResponses { get; set; }
}
public class CourtSubSettingResponseV2
{
    public Guid CourtSubSettingId { get; set; }
    public Guid SportCategoryId { get; set; }
    public string? TypeSize { get; set; }
    public string? SportCategoryName { get; set; }
    public string? ShortName { get; set; }
    public List<ListCourtSubdivisionAndTimeDataByCourtSubdivisionId> CourtSubdivision { get; set; }
}