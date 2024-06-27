using System.ComponentModel.DataAnnotations.Schema;
using BeatSportsAPI.Domain.Entities.CourtEntity;

namespace BeatSportsAPI.Domain.Entities.Room;
public class RoomMatch : BaseAuditableEntity
{
    [ForeignKey("Booking")]
    public Guid BookingId { get; set; }
    [ForeignKey("Level")]
    public Guid LevelId { get; set; }
    public TimeSpan StartTimeRoom { get; set; }
    public TimeSpan EndTimeRoom { get; set; }
    public int MaximumMember { get; set; }
    public string? RoomName { get; set; }
    public string? RuleRoom { get; set; }
    public string? Note { get; set; }

    public virtual Booking Booking { get; set; } = null!;
    public virtual Level Level { get; set; } = null!;
    public virtual IList<RoomMember> RoomMembers { get; set; } = null!;
    public virtual IList<RoomRequest> RoomRequests { get; set; } = null!;   
}
