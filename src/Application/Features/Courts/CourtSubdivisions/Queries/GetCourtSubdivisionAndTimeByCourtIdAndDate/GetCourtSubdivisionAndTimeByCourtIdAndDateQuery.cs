using MediatR;

namespace BeatSportsAPI.Application.Features.Courts.CourtSubdivisions.Queries.GetCourtSubdivisionAndTimeByCourtIdAndDate;
public class GetCourtSubdivisionAndTimeByCourtIdAndDateQuery : IRequest<CourtSubdivisionAndTime>
{
    public Guid CourtId { get; set; }
    public DateTime DateCheck { get; set; }
}
public class CourtSubdivisionAndTime
{
    public string CourtId { get; set; } = null!;
    public string CourtName { get; set; } = null!;
    public List<ListCourtSubdivisionAndTimeDataByCourtSubdivisionId>? MiniCourt { get; set; }
}
public class ListCourtSubdivisionAndTimeDataByCourtSubdivisionId
{
    public string CourtSubdivisionId { get; set; } = null!;
    public string NameMiniCourt { get; set; } = null!;
    public List<ListTimeCheckingByCourtSubdivisionId>? TimeListBooked { get; set; }

}
public class ListTimeCheckingByCourtSubdivisionId
{
    public string TimeCheckingId { get; set; } = null!;
    public string StartTimeBooking { get; set; } = null!;
    public string EndBooking { get; set; } = null!;
}