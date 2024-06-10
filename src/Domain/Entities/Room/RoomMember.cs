using System.ComponentModel.DataAnnotations.Schema;

namespace BeatSportsAPI.Domain.Entities.Room;
public class RoomMember : BaseAuditableEntity
{
    [ForeignKey("Customer")]
    public Guid CustomerId { get; set; }
    [ForeignKey("RoomMatch")]
    public Guid RoomMatchId { get; set; }
    public string? RoleInRoom { get; set; }

    public virtual Customer Customer { get; set; } = null!;
    public virtual RoomMatch RoomMatch { get; set; } = null!;
}
