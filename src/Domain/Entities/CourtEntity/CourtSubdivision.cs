using System.ComponentModel.DataAnnotations.Schema;
using BeatSportsAPI.Domain.Entities.Room;

namespace BeatSportsAPI.Domain.Entities.CourtEntity;
public class CourtSubdivision : BaseAuditableEntity
{
    [ForeignKey("Court")]
    public Guid CourtId { get; set; }
    public string? Description { get; set; }
    public string? ImageURL { get; set; }
    public bool IsActive { get; set; }
    public decimal BasePrice { get; set; }
    public string? CourtSubdivisionName { get; set; }

    public virtual Court Court { get; set; }
    public virtual IList<RoomMatch> RoomMatch { get; set; } = null!;
    public virtual Booking Bookings { get; set; } = null!;
    public virtual IList<TimePeriod>? TimePeriods { get; set; }
}
