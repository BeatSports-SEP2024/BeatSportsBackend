namespace BeatSportsAPI.Domain.Entities.CourtEntity.TimePeriod;
public class TimePeriodCourtSubdivision
{
    public Guid CourtSubdivisionId { get; set; }
    public Guid TimePeriodId { get; set; }

    public virtual CourtSubdivision CourtSubdivision { get; set; } = null!;
    public virtual TimePeriod TimePeriod { get; set; } = null!;
}
