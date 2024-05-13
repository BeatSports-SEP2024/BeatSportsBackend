using System.ComponentModel.DataAnnotations.Schema;

namespace BeatSportsAPI.Domain.Entities.CourtEntity;
public class CourtTimePeriod
{
    [ForeignKey("Court")]
    public Guid CourtId { get; set; }
    [ForeignKey("TimePeriod")]
    public Guid TimePeriodId { get; set; }

    public virtual Court Court { get; set; }
    public virtual TimePeriod TimePeriod { get; set; }
}
