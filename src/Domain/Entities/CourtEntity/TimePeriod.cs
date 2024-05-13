namespace BeatSportsAPI.Domain.Entities.CourtEntity;
public class TimePeriod : BaseAuditableEntity
{
    public string Description { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public decimal RateMultiplier { get; set; }

    public virtual IList<CourtTimePeriod> CourtTimePeriods { get; set; }
}
