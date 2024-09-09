using System.ComponentModel.DataAnnotations.Schema;
using BeatSportsAPI.Domain.Entities.CourtEntity;

namespace BeatSportsAPI.Domain.Entities.Room;
public class RoomMatch : BaseAuditableEntity
{
    [ForeignKey("Booking")]
    public Guid BookingId { get; set; }
    [ForeignKey("Level")]
    public Guid LevelId { get; set; }
    [ForeignKey("RatingRoom")]
    public Guid? RatingRoomId { get; set; }
    public SportCategoriesEnums SportCategory { get; set; }
    public DateTime StartTimeRoom { get; set; }
    public DateTime EndTimeRoom { get; set; }
    public string? RoomMatchTypeName { get; set; }
    public int MaximumMember { get; set; }
    public string? RoomName { get; set; }
    public double? TotalCostEachMember { get; set; }
    public string? RuleRoom { get; set; }
    public string? Note { get; set; }
    public bool? IsPrivate { get; set; }


    public virtual RatingRoom? RatingRoom { get; set; }
    public virtual Booking Booking { get; set; } = null!;
    public virtual Level Level { get; set; } = null!;
    public virtual IList<RoomMember> RoomMembers { get; set; } = null!;
    public virtual IList<RoomRequest> RoomRequests { get; set; } = null!;   
}
