using System.ComponentModel.DataAnnotations.Schema;

namespace BeatSportsAPI.Domain.Entities.CourtEntity;
public class TimePeriod : BaseAuditableEntity
{
    [ForeignKey("CourtSubdivision")]
    public Guid CourtSubdivisionId { get; set; }
    public string? Description { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public decimal? RateMultiplier { get; set; }
    public bool IsRefundDeposit { get; set; }
    public decimal? PercentDeposit { get; set; }

    public virtual CourtSubdivision CourtSubdivision { get; set; }
}
