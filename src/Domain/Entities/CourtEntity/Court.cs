using System.ComponentModel.DataAnnotations.Schema;

namespace BeatSportsAPI.Domain.Entities.CourtEntity;
public class Court : BaseAuditableEntity
{
    [ForeignKey("Owner")]
    public Guid OwnerId { get; set; }
    public int Capacity { get; set; }
    public string? Description { get; set; }
    public string? CourtName { get; set; }
    public TimeSpan TimeStart { get; set; }
    public TimeSpan TimeEnd { get; set; }

    public virtual Owner Owner { get; set; }
    public virtual IList<CourtSportCategory> CourtCategories { get; set; }
    public virtual IList<CourtTimePeriod> CourtTimePeriods { get; set; }
    public virtual IList<BookingDetail> BookingDetails { get; set; }
}
