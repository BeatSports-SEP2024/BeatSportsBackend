using System.ComponentModel.DataAnnotations.Schema;
using BeatSportsAPI.Domain.Entities.CourtEntity;
using BeatSportsAPI.Domain.Entities.Room;

namespace BeatSportsAPI.Domain.Entities;
public class Customer : BaseAuditableEntity
{
    [ForeignKey("Account")]
    public Guid AccountId { get; set; }
    public int RewardPoints { get; set; } = 0;
    public string? Address { get; set; }

    public virtual Account Account { get; set; } = null!;
    public virtual IList<Booking>? Bookings { get; set; }
    public virtual IList<RoomMember>? RoomMembers { get; set; }
}
